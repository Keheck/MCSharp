using CommandLine;

namespace MCSharp.Options;

[Verb("build", true)]
class CompileOptions {
    private string _inputFile = "";
    [Option('i', "infile", SetName = "build")]
    public string InputFile { 
        get { return _inputFile; } 
        set {
            if(value == null) { throw new ArgumentException("Input File must be set!"); }
            _inputFile = value;
        } 
    }

    [Option('w', "log-warnings", SetName = "build")]
    public bool Warn { get; set; } = true;

    [Option('t', "token", SetName = "build")]
    public string API_Token { get; set; } = "";
}