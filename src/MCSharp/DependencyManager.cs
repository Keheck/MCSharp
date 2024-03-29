using System.Xml;
using Octokit;
using MCSharp.Core.Options;
using System.IO.Compression;

namespace MCSharp.Core;

class DependencyManager {
    private static readonly object objLock = new object();
    private static string path = Directory.GetCurrentDirectory();

    private DependencyManager() {}

    public static int Resolve(DependencyOptions options) {
        if(options.Path != null)
            path = options.Path;

        XmlNode document = Program.GetCompilerSettingsDocument(Path.Combine(path, "mcsharp.xml"))["compiler"]!;

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

        XmlNode? node = dependency["release"] ?? dependency["commit"];
        string version = node != null ? node.InnerText : "HEAD";

        byte[] content = await client.Repository.Content.GetArchive(author, repoName, ArchiveFormat.Zipball, version);

        string zipFileName = Path.Combine(path, "dependencies", "temp", $"{author}_{repoName}_{version}.zip)");
        Directory.CreateDirectory(Path.Combine(path, "dependencies", "temp"));
        FileStream stream = new FileStream(zipFileName, System.IO.FileMode.OpenOrCreate, FileAccess.Write);
        await stream.WriteAsync(content, 0, content.Length);
        stream.Close();

        ZipFile.ExtractToDirectory(zipFileName, Path.Combine(path, "dependencies"));
    }
}