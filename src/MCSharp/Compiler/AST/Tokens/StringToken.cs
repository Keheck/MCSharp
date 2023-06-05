namespace MCSharp.Compiler.AST;

using System.Text.RegularExpressions;

public class StringToken {
    public static readonly Regex STRING_REGEX = new Regex(@""".*?(?<!\)""");
    public string Value { get; }

    public StringToken(string value) {
        Value = value;
    }
}