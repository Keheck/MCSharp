using CommandLine;
using System.Xml;
using System.IO;
using MimeTypes;
using System.Reflection;

using MCSharp.Options;

namespace MCSharp;

class Program {
    public static readonly Assembly assembly = typeof(Program).Assembly;

    static void Main(string[] args) {
        var result = Parser.Default.ParseArguments<CompileOptions>(args).Value;
        string inputPath = result.InputFile;
    
        if(MimeTypeMap.GetMimeType(inputPath.Split('.').Last()) != MimeTypeMap.GetMimeType("xml"))
            throw new ArgumentException("File must have the XML mime type");
        
        XmlDocument document = new XmlDocument();
        document.Load(new FileStream(inputPath, FileMode.Open));

        
    }
}
