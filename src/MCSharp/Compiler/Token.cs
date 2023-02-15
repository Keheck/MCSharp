namespace MCSharp.Compiler;

class Token {

    public enum TokenType {
        RESERVED,
        INTEGER_LITERAL,
        STRING_LITERAL,
        MULTILINE_LITERAL,
        NBT_LITERAL,
        IDENTIFIER,
        PREPROCESSOR_DIRECTIVE,
        BOOL_OPERATOR,
        ARITHMETIC_OPERATOR,
        ASSIGNMENT_OPERATOR
    }
}