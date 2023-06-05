namespace MCSharp.Compiler.AST;

using System.Text.RegularExpressions;

public class FloatNode {
    public static readonly Regex FLOAT_REGEX = new Regex(@"[-+]?\d+\.\d+");
    public float Number { get; }

    public FloatNode(float number) {
        Number = number;
    }
}