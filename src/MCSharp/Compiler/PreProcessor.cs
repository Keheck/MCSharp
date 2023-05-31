namespace MCSharp.Compiler;

using System.IO;
using System.Collections.Immutable;
using System.Xml;

class PreProcessor {
    private static Dictionary<string, string> _globalMacros = new Dictionary<string, string>();

    public static ImmutableDictionary<string, string> GlobalMacros { get { return ImmutableDictionary.ToImmutableDictionary(_globalMacros); }}

    public static void init(XmlDocument document) {
        XmlElement? definitions = document["compiler"]!["defines"];
        if(definitions == null) return;

        foreach(XmlElement define in definitions.GetElementsByTagName("define")) {
            Logger.log($"{define.Attributes["name"]!.Value}: {define.Attributes["value"]?.Value ?? "1"}", Logger.LogLevel.INFO);
        }
    }
}