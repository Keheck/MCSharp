namespace MCSharp.Compiler.AST;

using System.Text.RegularExpressions;

public class IdentifierNode {
    public static readonly Regex IDENTIFIER_REGEX = new Regex(@"[A-z_]\w*");
    public string Value { get; }

    public IdentifierNode(string value) {
        Value = value;
    }
}