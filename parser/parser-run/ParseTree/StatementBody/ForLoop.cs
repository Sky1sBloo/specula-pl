namespace SpeculaSyntaxAnalyzer.ParseTree;

public interface ForInit;
public record ForLoop(ForInit Init, Expression Expression, Assignment Assignment, BodyNode Body) : Statement;
