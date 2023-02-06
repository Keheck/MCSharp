using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Xml;

namespace MCSharp;

/* This is a utility class for easy comparison of Minecraft version numbers and other things*/
class MinecraftVersion {
    public static readonly Regex VERSION_REGEX = new Regex(@"^\d+\.\d+(\.\d+)?$");

    private static Dictionary<int, string> versionMap = new Dictionary<int, string>();

    private MinecraftVersion() {}

    public static int Compare(string version, string other) {
        if(!VERSION_REGEX.Match(version).Success || !VERSION_REGEX.Match(other).Success) 
            throw new ArgumentException("One or both provided versions did not match the conventional versioning system");
        
        int[] versionNumbers = version.Split('.').Select(s => int.Parse(s)).ToArray();
        int[] otherVersionNumbers = other.Split('.').Select(s => int.Parse(s)).ToArray();

        for(int i = 0; i < Math.Min(versionNumbers.Length, otherVersionNumbers.Length); i++) {
            if(versionNumbers[i] != otherVersionNumbers[i])
                return versionNumbers[i] - otherVersionNumbers[i];
        }

        return versionNumbers.Length - otherVersionNumbers.Length;
    }

    private static void initialiseVersionMap() {
        Stream? versionInfoStream = Program.assembly.GetManifestResourceStream("MCSharp.data.metadata.xml");

        if(versionInfoStream == null)
            throw new FileNotFoundException("Could not find information on Minecraft versions");
        
        XmlDocument document = new XmlDocument();
        document.Load(versionInfoStream);
        // Shut up, I know what I'm doing
        #pragma warning disable CS8600, CS8602
        XmlNode versionsRoot = document["minecraft"]["version-packformat-map"];

        foreach(XmlNode entry in versionsRoot.ChildNodes) {
            int packFormat = int.Parse(entry["pack-format"].InnerText);
            string version = entry["version"].InnerText;

            versionMap[packFormat] = version;
        }
        #pragma warning restore
    }

    public static string getMinVersion(int format) {
        if(versionMap.Count == 0) 
            initialiseVersionMap();

        return versionMap[format];
    }
}