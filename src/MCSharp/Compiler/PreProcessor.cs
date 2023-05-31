namespace MCSharp.Compiler;

using System.IO;
using System.Collections.Immutable;
using System.Xml;

class PreProcessor {
    // Store object-like macros
    private static Dictionary<string, string> _globalObjectMacros = new Dictionary<string, string>();
    public static ImmutableDictionary<string, string> GlobalObjectMacros { get { return ImmutableDictionary.ToImmutableDictionary(_globalObjectMacros); }}

    //Store function-like macros. First element of the value
    private static Dictionary<string, string> _globalFunctionMacros = new Dictionary<string, string>();
    public static ImmutableDictionary<string, string> GlobalFunctionMacros { get {return ImmutableDictionary.ToImmutableDictionary(_globalFunctionMacros); }}

    public static void init(XmlDocument document) {
        XmlElement? definitions = document["compiler"]!["defines"];
        if(definitions == null) return;

        foreach(XmlElement define in definitions.GetElementsByTagName("define")) {
            string name = definitions.Attributes["name"]!.Value;
        }
    }
}
