namespace MCSharp.Compiler;

using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

class PreProcessor {
    private const uint STRING =                0b_0000_0001;
    private const uint LINE_COMMENT =          0b_0000_0010;
    private const uint BLOCK_COMMENT =         0b_0000_0100;
    private const uint ENCOUNTERED_BACKSLASH = 0b_0000_1000;

    private static uint codeContext = 0;

    public static void Init(XmlDocument document) {
        
    }

    public static string Process(string input, bool trimComments) {
        StringBuilder builder = new StringBuilder();

        if(trimComments) 
            TrimComments(ref input);

        // No need to process empty lines, nor do we need trailing/leading spaces in the source code -> Remove them
        // input = Regex.Replace(input, @"\n{2,}", "\n").Trim();

        builder.Append(input);
        return builder.ToString();
    }

    private static bool IsCurrentContextAllOf(params uint[] partialContexts) {
        uint totalContext = partialContexts.Aggregate((aggregate, element) => aggregate | element);
        return (codeContext & totalContext) == totalContext;
    }

    private static bool IsCurrentContextAnyOf(params uint[] partialContexts) {
        uint totalContext = partialContexts.Aggregate((aggregate, element) => aggregate | element);
        return (codeContext & totalContext) != 0;
    }

    private static bool IsCurrentContextOnlyOf(params uint[] partialContexts) {
        uint invertedTotalContext = ~partialContexts.Aggregate((aggregate, element) => aggregate | element);
        return IsCurrentContextAllOf(partialContexts) && !IsCurrentContextAnyOf(invertedTotalContext);
    }

    private static void ToggleCurrentContext(uint contextMask) {
        codeContext ^= contextMask;
    }

    private static void SetCurrentContext(uint contextMask) {
        codeContext |= contextMask;
    }

    private static void UnsetCurrentContext(uint contextMask) {
        codeContext &= ~contextMask;
    }

    private static void TrimComments(ref string input) {
        StringBuilder builder = new StringBuilder();

        int startInclusive = 0;
        int endExclusive = 0;
        codeContext = 0;

        for(int i = 0; i < input.Length; i++) {
            //We don't want to parse the current character in a string if it's escaped, as it will have its meaning as a normal character instead of a real token"
            if(IsCurrentContextAllOf(STRING, ENCOUNTERED_BACKSLASH)) {
                ToggleCurrentContext(ENCOUNTERED_BACKSLASH);
                continue;
            }

            char c = input[i];

            switch(c) {
                case '\n':
                    if(IsCurrentContextAllOf(STRING))
                        throw new CompilerException("Unexpected line break inside string", "", input, i);
                    if(IsCurrentContextAllOf(LINE_COMMENT)) { 
                        startInclusive = i+1;
                        ToggleCurrentContext(LINE_COMMENT);
                    }
                    break;
                case '"':
                    if(!IsCurrentContextAnyOf(LINE_COMMENT, BLOCK_COMMENT))
                        ToggleCurrentContext(STRING);
                    break;
                case '\\':
                    if(IsCurrentContextAnyOf(STRING))
                        SetCurrentContext(ENCOUNTERED_BACKSLASH);
                    break;
                case '/':
                    //if((codeContext & (STRING | ANY_COMMENT)) == 0 && input[i+1] == '/') {
                    if(!IsCurrentContextAnyOf(STRING, LINE_COMMENT, BLOCK_COMMENT) && input[i+1] == '/') {
                        SetCurrentContext(LINE_COMMENT);
                        endExclusive = i;
                    }
                    //else if((codeContext & (STRING | ANY_COMMENT)) == 0 && input[i+1] == '*') {
                    else if(!IsCurrentContextAnyOf(STRING, LINE_COMMENT, BLOCK_COMMENT) && input[i+1] == '*') {
                        SetCurrentContext(BLOCK_COMMENT);
                        endExclusive = i;
                    }
                    //else if((codeContext & (STRING | BLOCK_COMMENT)) == BLOCK_COMMENT && input[i-1] == '*') {
                    else if(IsCurrentContextAnyOf(STRING, LINE_COMMENT, BLOCK_COMMENT) && input[i-1] == '*') {
                        ToggleCurrentContext(BLOCK_COMMENT);
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
        
            if(IsCurrentContextAnyOf(STRING))
                message = "Unexpected EOF in string";
            else if(IsCurrentContextAnyOf(BLOCK_COMMENT))
                message = "Unexpected EOF in block comment";
            
            throw new CompilerException(message, "", input.Count(c => c == '\n'), input.Length - input.LastIndexOf('\n'));
        }

        input = builder.ToString();
    }
}
