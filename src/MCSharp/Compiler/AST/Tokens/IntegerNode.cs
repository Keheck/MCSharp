namespace MCSharp.Compiler.AST;

using System.Text.RegularExpressions;

public class IntegerNode {
    public static readonly Regex NUMBER_REGEX = new Regex(@"[-+]?\d+");
    public int Number { get; }

    public IntegerNode(int number) {
        Number = number;
    }
}