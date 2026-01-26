namespace SpeculaSyntaxAnalyzer.ParseTree;

public record BodyNode(PrintableList<Statement> Statements) : Statement;
