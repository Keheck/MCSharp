namespace MCSharp.Compiler.AST;

using System.Collections.Immutable;

public class SourceRootNode {
    private List<StatementNode> _statements = new List<StatementNode>();
    public ImmutableList<StatementNode> Statements { get { return _statements.ToImmutableList(); }}
    public SourceRootNode() {

    }
}