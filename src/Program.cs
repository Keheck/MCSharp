using CommandLine;
using System.IO;
using MimeTypes;

class Program {
  static void Main(string[] args) {
    Options result = Parser.Default.ParseArguments<Options>(args).Value;
    string inputPath = result.InputFile;
    
    if(MimeTypeMap.GetMimeType(inputPath.Split('.').Last()) != "application/xml")
      throw new ArgumentException("File must have the XML mime type");
    
    
  }
}
