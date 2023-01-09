using CommandLine;

class Options {
  private string _inputFile = "";
  [Value(0)]
  public string InputFile { 
    get { return _inputFile; } 
    set {
      if(value == null) { throw new ArgumentException("Input File must be set!"); }
      _inputFile = value;
    } 
  }

  [Option('w', "log-warnings")]
  public bool Warn { get; set; } = true;
}