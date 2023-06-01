namespace MCSharp.Compiler;

using System.Xml;
using System.Text;

class PreProcessor {
    private const uint STRING =             0b_0000_0001;
    private const uint LINE_COMMENT =       0b_0000_0010;
    private const uint BLOCK_COMMENT =      0b_0000_0100;
    private const uint ENCOUNTERED_ESCAPE = 0b_0000_1000;

    private const uint ANY_COMMENT = LINE_COMMENT | BLOCK_COMMENT;

    public static void Init(XmlDocument document) {
        
    }

    public static string Process(string input, bool trimComments) {
        StringBuilder builder = new StringBuilder();
        int i = 0;

        if(trimComments) 
            TrimComments(ref input);

        return builder.ToString();
    }

    private static void TrimComments(ref string input) {
        StringBuilder builder = new StringBuilder();

        int start = 0;
        int end = 0;
        uint codeContext = 0;

        for(int i = 0; i < input.Length; i++) {
            char c = input[i];

            switch(c) {
                case '\n':
                    if((codeContext & STRING) != 0)
                        throw new CompilerException("Unexpected line break inside string", "", input, i);
                    if((codeContext & LINE_COMMENT) != 0)
                        end = i+1;
                    break;
                case '"':
                    if((codeContext & ANY_COMMENT) != 0)
                        codeContext ^= STRING;
                    break;
            }

            if(start < end)
                builder.Append(input.Substring(start, end-start));
        }

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
