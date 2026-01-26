namespace SpeculaSyntaxAnalyzer.ParseTree;

public record RootNode(PrintableList<RootStatement> Statements) : ParseNode;
