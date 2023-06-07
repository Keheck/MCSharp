namespace MCSharp.Compiler.AST;

using System.Text.RegularExpressions;
using System.Collections.Immutable;

public class IntegerLiteralNode {
    //Regex for a base-10 integer
    public static readonly Regex INTEGER_LITERAL10_REGEX = new Regex(@"[-+]?[0-9]+");
    //Rexgex for a base-16 integer
    public static readonly Regex INTEGER_LITERAL16_REGEX = new Regex(@"([-+])?0x([0-9a-fA-F]+)");
    //Regex for a base-2 integer
    public static readonly Regex INTEGER_LITERAL2_REGEX = new Regex(@"([-+])?0b([01]{1,31})");
    public int Value { get; }

    public IntegerLiteralNode(string valueAsString) {
        if(INTEGER_LITERAL10_REGEX.IsMatch(valueAsString)) {
            Value = int.Parse(valueAsString);
        }
        else if(INTEGER_LITERAL16_REGEX.IsMatch(valueAsString)) {
            Match base16Match = INTEGER_LITERAL16_REGEX.Match(valueAsString);

            // True if positive, false if negative
            bool sign = base16Match.Groups[1].Value != "-";
            int abs = int.Parse(base16Match.Groups[2].Value, System.Globalization.NumberStyles.HexNumber);
            
            Value = sign ? abs : -abs;
        }
        else if(INTEGER_LITERAL2_REGEX.IsMatch(valueAsString)) {
            Match base2Match = INTEGER_LITERAL2_REGEX.Match(valueAsString);

            bool sign = base2Match.Groups[1].Value != "-";
            int abs = 0;

            foreach(char c in valueAsString) {
                abs *= 2;

                if(c == '1')
                    abs++;
            }

            Value = sign ? abs : -abs;
        }
    }
}