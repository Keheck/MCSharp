using CommandLine;
using System.Xml;
using MimeTypes;
using System.Reflection;
using System.Web;
using System.Text;
using Octokit;

using MCSharp.Options;

namespace MCSharp;

class Program {
    public static readonly Assembly assembly = typeof(Program).Assembly;

    private static readonly object objLock = new object();

    static void Main(string[] args) {
        var result = Parser.Default.ParseArguments<CompileOptions, DependencyOptions>(args)
            .MapResult(
                (CompileOptions options) => CompileProject(options),
                (DependencyOptions options) => ResolveDependencies(options),
                (errors) => 1
            );
    }

    private static int CompileProject(CompileOptions options) {
        string inputPath = options.InputFile;

        if(MimeTypeMap.GetMimeType(inputPath.Split('.').Last()) != MimeTypeMap.GetMimeType("xml"))
            throw new ArgumentException("File must have the XML mime type");
        
        XmlDocument document = new XmlDocument();
        document.Load(new FileStream(inputPath, System.IO.FileMode.Open));

        return 0;
    }

    private static int ResolveDependencies(DependencyOptions options) {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("MCSharp-Compiler", "1.0"));

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
                    Console.WriteLine("A 401 - Unauthorised error has occured. Did you provide an invalid token?");
                    Console.WriteLine(e.Message);
                    return 1;
                }
            }
        }

        return 0;
    }
}
