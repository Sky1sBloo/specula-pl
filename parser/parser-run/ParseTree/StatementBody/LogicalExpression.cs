namespace SpeculaSyntaxAnalyzer.ParseTree;

public interface Logical : Expression;

public record AndLogical(Expression Lhs, Expression Rhs) : Logical;
public record OrLogical(Expression Lhs, Expression Rhs) : Logical;
public record NotLogical(Expression Node) : Logical;
