using System.Xml;
using Octokit;
using MCSharp.Options;

namespace MCSharp;

class DependencyManager {
    private static readonly object objLock = new object();

    private DependencyManager() {}

    public static int Resolve(DependencyOptions options) {
        XmlDocument document = Program.GetCompilerSettingsDocument(options.CompilerDirectory);

        if(options.Fetch) {
            XmlNode? dependenciesNode = document["dependencies"];

            if(dependenciesNode == null) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No dependencies were referenced, fetching is unnecessary. Skipping...");
                Console.ResetColor();
            }
            else {
                List<Task> fetchTasks = new List<Task>();

                foreach(XmlNode dependency in dependenciesNode.ChildNodes) 
                    lock(objLock) { 
                    Task task = Task.Run(async () => await FetchDependency(dependency));
                    fetchTasks.Add(task); 
                }

                while(fetchTasks.Count > 0)
                    fetchTasks = fetchTasks.Where(t => !t.IsCompleted).ToList();
            }
        }

        return 0;
    }

    private static async Task FetchDependency(XmlNode? dependency) {
        if(dependency == null) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No dependencies were referenced, skipping dependency...");
            return;
        }

        GitHubClient client = Program.client;

        string author = dependency["author"]!.InnerText;
        string repoName = dependency["repository"]!.InnerText;
        string version = dependency["version"] == null ? "LATEST" : dependency["version"]!.InnerText;

        Repository repository = await client.Repository.Get(author, repoName);

        repository.Url
    }
}