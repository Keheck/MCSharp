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

    private static readonly object objLock = new object();

    static void Main(string[] args) {
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

    public static XmlDocument GetCompilerSettingsDocument(string? location) {
        XmlDocument document = new XmlDocument();

        try { 
            FileInfo compilerDefinition = new DirectoryInfo(location ?? Directory.GetCurrentDirectory()).EnumerateFiles().Where((info) => info.Name == "mcsharp.xml").Single();
            document.Load(compilerDefinition.Open(System.IO.FileMode.Open));

            //XmlSchema schema = XmlSchema.Read();
        }
        catch(InvalidOperationException) {
            throw new FileNotFoundException($"No file named mcsharp.xml could be found in {location}. It is a required file for setting up the compiler.");
        }

        return document;
    }

    // Find a way to check the up-to-dateness of the xml schema definition without too many API calls
    private async static Task FetchSchemaDefinition() {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("MCSharp-Compiler", "1.0"));
        client.Credentials = new Credentials("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");
        FileInfo settings = new FileInfo("CompilerSettingsDefinition.xsd");

        if(!settings.Exists) {
            RepositoryContent content = (await client.Repository.Content.GetAllContents("Keheck", "MCSharp", "CompilerSettingsDefinition.xsd")).Single();
        }
    }

    private static int CompileProject(CompileOptions options) {
        XmlDocument document = GetCompilerSettingsDocument(options.InputFile);

        return 0;
    }

    private static int ResolveDependencies(DependencyOptions options) {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("MCSharp-Compiler", "1.0"));

        XmlDocument document = GetCompilerSettingsDocument(options.CompilerDirectory);

        if(options.AccessToken == "") {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Attention: You have not passed an access token as a parameter. Without authentication, Github restricts the requests " + 
            "the number of API calls to 60 requests/hour, whereas an authentication token would provide you with 5000. " +
            "You can pass a token to the MCSharp compiler by using the -t or --token option. Do you wish to continue without a token anyway? [y/N]");

            string confirm = Console.ReadLine() ?? "N";

            if(confirm.ToUpper().First() == 'Y')
                return 0;
            
            Console.WriteLine("Continuing without token, rate limit is expected to be 60 requests/hour...");
            Console.ResetColor();
        }
        else {
            client.Credentials = new Credentials(options.AccessToken);

            MiscellaneousRateLimit rateLimit;

            // We don't want to work with invalid credentials don't we?
            lock(objLock) {
                Console.WriteLine("Checking credentials...");

                try { 
                    var task = Task.Run(async () => await client.RateLimit.GetRateLimits()); 
                    task.Wait();
                    rateLimit = task.Result;
                }
                catch(AuthorizationException e) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                    return 1;
                }
            }
        }

        if(options.Fetch) {
            XmlNode? dependenciesNode = document["dependencies"];

            if(dependenciesNode == null) {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
        }

        return 0;
    }
}
