namespace MCSharp.Compiler;

public class CompilerException: Exception
{
    public int Line { get; }
    public int Column { get; }
    public string SourceFile { get; }

    public CompilerException(string reason, string sourceFile, int line, int column): base(reason) {
        Line = line;
        Column = column;
        SourceFile = sourceFile;
    }

    public CompilerException(string reason, string sourceFile, string sourceCode, int atCharacter): base(reason) {
        string fromStartToError = sourceCode.Substring(0, atCharacter);

        Line = fromStartToError.Count(c => c == '\n');
        Column = fromStartToError.Length - fromStartToError.LastIndexOf('\n');
        SourceFile = sourceFile;
    }

    public override string ToString() {
        return $"Compilation error on line {Line} and column {Column} in {SourceFile}: {Message}";
    }
}