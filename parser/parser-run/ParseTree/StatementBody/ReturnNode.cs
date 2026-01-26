namespace SpeculaSyntaxAnalyzer.ParseTree;

public record ReturnNode(Expression? ReturnValue) : Statement;