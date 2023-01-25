using System.Xml;
using Octokit;
using MCSharp.Options;

namespace MCSharp;

class DependencyManager {
    private static readonly object objLock = new object();

    private DependencyManager() {}

    public static int Resolve(DependencyOptions options) {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("MCSharp-Compiler", "1.0"));

        XmlDocument document = Program.GetCompilerSettingsDocument(options.CompilerDirectory);

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
            if(document["dependencies"] == null) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No dependencies were referenced, fetching is unnecessary. Skipping...");
                Console.ResetColor();
            }
            else {
                foreach(XmlNode dependency in document["dependencies"].ChildNodes) 
                    FetchDependency(dependency);
            }
        }

        return 0;
    }

    private static void FetchDependency(XmlNode? dependency) {
        if(dependency == null) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No dependencies were referenced, skipping dependency...");
            return;
        }
    }
}