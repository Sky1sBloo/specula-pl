namespace SpeculaSyntaxAnalyzer.ParseTree;

public interface Comparison : Expression;

public record EqCompExpression(Expression Lhs, Expression Rhs) : Comparison;
public record NotEqCompExpression(Expression Lhs, Expression Rhs) : Comparison;
public record GtCompExpression(Expression Lhs, Expression Rhs) : Comparison;
public record LtCompExpression(Expression Lhs, Expression Rhs) : Comparison;
public record GteCompExpression(Expression Lhs, Expression Rhs) : Comparison;
public record LteCompExpression(Expression Lhs, Expression Rhs) : Comparison;
