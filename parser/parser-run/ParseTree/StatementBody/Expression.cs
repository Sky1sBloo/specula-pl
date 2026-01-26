namespace SpeculaSyntaxAnalyzer.ParseTree;

public interface Expression : Statement;


public record AddExpression(Expression Lhs, Expression Rhs) : Expression;
public record SubExpression(Expression Lhs, Expression Rhs) : Expression;
public record MultExpression(Expression Lhs, Expression Rhs) : Expression;
public record DivExpression(Expression Lhs, Expression Rhs) : Expression;
public record PostIncExpression(Expression Node) : Expression;
public record PostDecExpression(Expression Node) : Expression;
public record PreIncExpression(Expression Node) : Expression;
public record PreDecExpression(Expression Node) : Expression;
public record PrePosExpression(Expression Node) : Expression;
public record PreNegExpression(Expression Node) : Expression;

