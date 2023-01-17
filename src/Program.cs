using CommandLine;
using System.IO;
using MimeTypes;
using System.Reflection;

class Program {
    public static readonly Assembly assembly = typeof(Program).Assembly;

    static void Main(string[] args) {
        Array.ForEach(assembly.GetManifestResourceNames(), Console.WriteLine);

        /*Options result = Parser.Default.ParseArguments<Options>(args).Value;
        string inputPath = result.InputFile;
    
        if(MimeTypeMap.GetMimeType(inputPath.Split('.').Last()) != "application/xml")
            throw new ArgumentException("File must have the XML mime type");*/
    }
}
