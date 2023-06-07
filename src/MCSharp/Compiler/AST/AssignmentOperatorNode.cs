namespace MCSharp.Compiler.AST;

using System.Text.RegularExpressions;

public class AssignmentOperatorNode {
    public static readonly Regex ASSIGNMENT_OPERATOR_REGEX = new Regex(@"(?:[-+*/%]?=)|[<>]|><");
}