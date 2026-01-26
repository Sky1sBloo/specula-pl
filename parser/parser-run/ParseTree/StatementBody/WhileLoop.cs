namespace SpeculaSyntaxAnalyzer.ParseTree;

public record WhileLoop(Expression Condition, BodyNode Body) : Statement;
public record DoWhileLoop(BodyNode Body, Expression Condition) : Statement;