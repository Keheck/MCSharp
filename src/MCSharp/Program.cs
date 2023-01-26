using CommandLine;
using System.Xml;
using System.Xml.Schema;
using MimeTypes;
using System.Reflection;
using System.Web;
using System.Text;
using Octokit;
using System.Security.Cryptography;

using MCSharp.Options;

namespace MCSharp;

class Program {
    public static readonly Assembly assembly = typeof(Program).Assembly;
    public static readonly GitHubClient client = new GitHubClient(new ProductHeaderValue("MCSharp-Compiler", "1.0"));

    private static readonly object objLock = new object();

    static void Main(string[] args) {
        EnvParser.LoadEnvIntoEnvironmentVariables();  
        
        if(Environment.GetEnvironmentVariable("GITHUB_TOKEN") == null) {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Attention: You have not given an API key to the .env file (GITHUB_TOKEN={key}). Without an API key, Github restricts requests to "+
            "60 requests per hour, whereas an API key allows for 5000 requests per hour. You can create an api token at https://github.com/settings/tokens?type=beta. "+
            "Do you wish to continue without an access token? [y/N]");

            string confirm = Console.ReadLine() ?? "N";

            if(confirm.ToUpper().First() != 'Y')
                return;
            
            Console.WriteLine("Continuing without token, rate limit is expected to be 60 requests/hour...");
            Console.ResetColor();
        }
        else {
            client.Credentials = new Credentials(Environment.GetEnvironmentVariable("GITHUB_TOKEN"));
        }

        lock(objLock) {
            Task task = Task.Run(async () => await FetchSchemaDefinition());
            task.Wait();
        }

        /*var result = Parser.Default.ParseArguments<CompileOptions, DependencyOptions>(args)
            .MapResult(
                (CompileOptions options) => CompileProject(options),
                (DependencyOptions options) => ResolveDependencies(options),
                (errors) => 1
            );*/
    }

    public static XmlDocument GetCompilerSettingsDocument(string ?location) {
        XmlDocument document = new XmlDocument();
        IEnumerable<FileInfo> files = new DirectoryInfo(location ?? Directory.GetCurrentDirectory()).EnumerateFiles();

        try { 
            FileInfo compilerDefinition = new DirectoryInfo(location ?? Directory.GetCurrentDirectory()).EnumerateFiles().Where((info) => info.Name == "mcsharp.xml").Single();
            document.Load(compilerDefinition.Open(System.IO.FileMode.Open));


        }
        catch(InvalidOperationException) {
            throw new FileNotFoundException($"No file named mcsharp.xml could be found in {location}. It is a required file for setting up the compiler.");
        }

        return document;
    }

    private async static Task FetchSchemaDefinition() {
        FileInfo settings = new FileInfo("CompilerSettingsDefinition.xsd");
        byte[] sha1 = SHA1.Create().ComputeHash(settings.OpenRead());

        if(!settings.Exists) {
            RepositoryContent content = (await client.Repository.Content.GetAllContents("Keheck", "MCSharp", "CompilerSettingsDefinition.xsd")).Single();
        }
    }

    private static int CompileProject(CompileOptions options) {
        XmlDocument document = GetCompilerSettingsDocument(options.InputFile);

        return 0;
    }

    private static int ResolveDependencies(DependencyOptions options) {
        XmlDocument document = GetCompilerSettingsDocument(options.CompilerDirectory);

        if(options.Fetch) {
            XmlNode? dependenciesNode = document["dependencies"];

            if(dependenciesNode == null) {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
        }

        return 0;
    }
}
