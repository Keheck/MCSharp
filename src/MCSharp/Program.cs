using CommandLine;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;
using Octokit;

using MCSharp.Core.Options;
using MCSharp.Compiler;

namespace MCSharp.Core;

class Program {
    public static readonly Assembly assembly = typeof(Program).Assembly;
    public static readonly GitHubClient client = new GitHubClient(new ProductHeaderValue("MCSharp-Compiler", "1.0"));
    public static string Cwd { get; private set; } = Directory.GetCurrentDirectory();

    private static readonly object objLock = new object();

    static int Main(string[] args) {
        EnvParser.LoadEnvIntoEnvironmentVariables();
        
        if(Environment.GetEnvironmentVariable("GITHUB_TOKEN") == null) {
            Logger.OverrideVerbosity = true;
            Logger.log("Attention: You have not given an access token to the .env file (GITHUB_TOKEN={key}). Without a token, Github restricts requests to "+
            "60 requests per hour, whereas an access token allows for 5000 requests per hour. You can create a token at https://github.com/settings/tokens?type=beta. "+
            "Do you wish to continue without an access token? [y/N]", Logger.LogLevel.WARN);

            string confirm = Console.ReadLine() ?? "N";

            if(confirm.ToUpper().First() != 'Y')
                return 0;
            
            Logger.log("Continuing without token, rate limit is expected to be 60 requests/hour...", Logger.LogLevel.WARN);
            Logger.OverrideVerbosity = false;
        }
        else {
            client.Credentials = new Credentials(Environment.GetEnvironmentVariable("GITHUB_TOKEN"));
        }

        int result = Parser.Default.ParseArguments<CompileOptions, DependencyOptions>(args)
            .MapResult(
                (CompileOptions options) => CompileProject(options),
                (DependencyOptions options) => DependencyManager.Resolve(options),
                (errors) => -1
            );
        
        return result;
    }

    public static XmlDocument GetCompilerSettingsDocument(string location) {
        XmlDocument document = new XmlDocument();

        try { 
            FileInfo compilerDefinition = new FileInfo(location);
            document.Load(compilerDefinition.Open(System.IO.FileMode.Open));

            XmlSchema schema = XmlSchema.Read(assembly.GetManifestResourceStream("MCSharp.data.CompilerSettingsSchema.xsd")!, null)!;
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(schema);

            document.Schemas = schemaSet;
            document.Validate((sender, e) => {
                throw new XmlSchemaValidationException("mcsharp.xml did not abide to the schema definition. Cannot continue compilation");
            });

            Console.WriteLine("Validated mcsharp.xml");
        }
        catch(InvalidOperationException) {
            throw new FileNotFoundException($"No file named mcsharp.xml could be found in {location}. It is a required file for setting up the compiler.");
        }

        return document;
    }

    private static int CompileProject(CompileOptions options) {
        if(options.InputFile != null) Directory.SetCurrentDirectory(options.InputFile);

        XmlDocument document = GetCompilerSettingsDocument(Path.Combine(Cwd, "mcsharp.xml"));
        PreProcessor.Init(document);

        foreach(string sourceFilePath in Directory.EnumerateFiles(Cwd, "*.func", SearchOption.AllDirectories)) {
            try {
                string sourceContent = File.ReadAllText(sourceFilePath);
                sourceContent = sourceContent.Replace("\r\n", "\n");
                Logger.log(PreProcessor.Process(sourceContent, true), Logger.LogLevel.INFO);
            } catch(CompilerException e) {
                Logger.log(e.Message, Logger.LogLevel.ERROR);
                return -1;
            }
        }

        return 0;
    }
}
