using System.Reflection.Metadata;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

/// <summary>
/// Used for recognizing variable assignment statements
/// </summary>
public class VarAssignHandler : Handler
{
    private readonly ExpressionHandler expressionHandler;
    public VarAssignHandler(ErrorsHandler errors) : base(errors)
    {
        expressionHandler = new(errors);
    }

    protected override ParseNode? verifyTokens()
    {
        if (CurrentToken.Type != Token.Types.IDENT)
        {
            throw new SyntaxErrorException(["IDENTIFIER"], CurrentToken);
        }
        string identifier = CurrentToken.Value;
        incrementIndex();
        switch (CurrentToken.Type)
        {
            case Token.Types.OP_EQUALS:
                incrementIndex();
                switch (CurrentToken.Type)
                {
                    case Token.Types.K_MOVE:
                        incrementIndex();
                        return new AssignmentMethodNode(identifier, AssignMethod.MOVE, handleMovableExpression());
                    case Token.Types.K_SHARE:
                        incrementIndex();
                        return new AssignmentMethodNode(identifier, AssignMethod.SHARE, handleMovableExpression());
                    case Token.Types.K_REF:
                        incrementIndex();
                        return new AssignmentMethodNode(identifier, AssignMethod.REF, handleMovableExpression());
                    default:
                        return new AssignmentStatementNode(identifier, handleExpressionAfterOperator());
                }
            case Token.Types.OP_PLUS_EQ:
                incrementIndex();
                return new AssignPlusEqNode(identifier, handleExpressionAfterOperator());
            case Token.Types.OP_MINUS_EQ:
                incrementIndex();
                return new AssignMinusEqNode(identifier, handleExpressionAfterOperator());
            case Token.Types.OP_MULT_EQ:
                incrementIndex();
                return new AssignMulEqNode(identifier, handleExpressionAfterOperator());
            case Token.Types.OP_DIV_EQ:
                incrementIndex();
                return new AssignDivEqNode(identifier, handleExpressionAfterOperator());
            case Token.Types.OP_MOD_EQ:
                incrementIndex();
                return new AssignModEqNode(identifier, handleExpressionAfterOperator());
            case Token.Types.OP_INCR:
                incrementIndex();
                return new AssignPlusEqNode(identifier, new LiteralValue(new TypeNode(DataTypes.INT), "1"));
            case Token.Types.OP_DECR:
                incrementIndex();
                return new AssignMinusEqNode(identifier, new LiteralValue(new TypeNode(DataTypes.INT), "1"));
        }

        return null;
    }
    private Expression handleExpressionAfterOperator()
    {
        ParseNode? value = delegateToHandler(expressionHandler);
        if (value is null || value is not Expression exprValue)
        {
            throw new SyntaxErrorException(["Expression"], CurrentToken);
        }
        return exprValue;
    }

    private Movable handleMovableExpression()
    {
        ParseNode? value = delegateToHandler(expressionHandler);
        if (value is null || value is not Expression exprValue)
        {
            throw new SyntaxErrorException(["Expression"], CurrentToken);
        }
        
        if (exprValue is not Movable movableVal)
        {
            throw new SyntaxErrorException(["identifier", "function call"], CurrentToken);
        }
        
        return movableVal;
    }
}