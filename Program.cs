using CommandLine;

class Program {
  static void Main(string[] args) {
    var result = Parser.Default.ParseArguments<Options>(args);
    Console.WriteLine(result.Value.Test);
  }
}
