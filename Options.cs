using CommandLine;

class Options {
  [Option('t', "test")]
  public string? Test { get; set; } = "World";
}