namespace MCSharp.Compiler;

using System.Xml;
using System.Text;
using System.Text.RegularExpressions;

class PreProcessor {
    private const uint STRING =                0b_0000_0001;
    private const uint LINE_COMMENT =          0b_0000_0010;
    private const uint BLOCK_COMMENT =         0b_0000_0100;
    private const uint ENCOUNTERED_BACKSLASH = 0b_0000_1000;

    private const uint ANY_COMMENT = LINE_COMMENT | BLOCK_COMMENT;

    public static void Init(XmlDocument document) {
        
    }

    public static string Process(string input, bool trimComments) {
        StringBuilder builder = new StringBuilder();

        if(trimComments) 
            TrimComments(ref input);

        // No need to process empty lines, nor do we need trailing/leading spaces in the source code -> Remove them
        input = Regex.Replace(input, @"\n{2,}", "\n").Trim();

        builder.Append(input);
        return builder.ToString();
    }

    private static void TrimComments(ref string input) {
        StringBuilder builder = new StringBuilder();

        int startInclusive = 0;
        int endExclusive = 0;
        uint codeContext = 0;

        for(int i = 0; i < input.Length; i++) {
            if((codeContext & (STRING | ENCOUNTERED_BACKSLASH)) == (STRING | ENCOUNTERED_BACKSLASH)) {
                codeContext ^= ENCOUNTERED_BACKSLASH;
                continue;
            }

            char c = input[i];

            switch(c) {
                case '\n':
                    if((codeContext & STRING) != 0)
                        throw new CompilerException("Unexpected line break inside string", "", input, i);
                    if((codeContext & LINE_COMMENT) != 0) { 
                        startInclusive = i+1;
                        codeContext ^= LINE_COMMENT;
                    }
                    break;
                case '"':
                    if((codeContext & ANY_COMMENT) == 0)
                        codeContext ^= STRING;
                    break;
                case '\\':
                    if((codeContext & STRING) != 0)
                        codeContext |= ENCOUNTERED_BACKSLASH;
                    break;
                case '/':
                    if((codeContext & (STRING | ANY_COMMENT)) == 0 && input[i+1] == '/') {
                        codeContext |= LINE_COMMENT;
                        endExclusive = i;
                    }
                    else if((codeContext & (STRING | ANY_COMMENT)) == 0 && input[i+1] == '*') {
                        codeContext |= BLOCK_COMMENT;
                        endExclusive = i;
                    }
                    else if((codeContext & (STRING | BLOCK_COMMENT)) == BLOCK_COMMENT && input[i-1] == '*') {
                        codeContext ^= BLOCK_COMMENT;
                        startInclusive = i+1;
                    }
                    break;
            }

            if(startInclusive < endExclusive) {
                builder.Append(input.Substring(startInclusive, endExclusive-startInclusive));
                startInclusive = endExclusive;
            }
        }

        builder.Append(input.Substring(startInclusive, input.Length-startInclusive));

        if(codeContext != 0) {
            string message = "";
        
            if((codeContext & STRING) != 0)
                message = "Unexpected EOF in string";
            else if((codeContext & BLOCK_COMMENT) != 0)
                message = "Unexpected EOF in block comment";
            
            throw new CompilerException(message, "", input.Count(c => c == '\n'), input.Length - input.LastIndexOf('\n'));
        }

        input = builder.ToString();
    }
}
