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
                foreach(XmlNode dependency in dependenciesNode.ChildNodes) 
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