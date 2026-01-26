using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

/// <summary>
/// Handles expression parsing with support for:
/// - Binary operators: +, -, *, /
/// - Comparison: ==
/// - Unary operators: ++, -- (pre and post)
/// - Bracket expressions
/// </summary>
public class ExpressionHandler : Handler
{
    private ValueHandler valueHandler;

    public ExpressionHandler(ErrorsHandler err) : base(err)
    {
        valueHandler = new ValueHandler(err);
    }

    protected override ParseNode? verifyTokens()
    {
        return ParseExpression();
    }

    /// <summary>
    /// Parses a complete expression (lowest precedence)
    /// </summary>
    private Expression ParseExpression()
    {
        return ParseLogicalOr();
    }

    /// <summary>
    /// Parses logical OR expressions (||)
    /// </summary>
    private Expression ParseLogicalOr()
    {
        Expression left = ParseLogicalAnd();

        while (HasMoreTokens && CurrentToken.Type == Token.Types.OP_OR)
        {
            incrementIndex();
            Expression right = ParseLogicalAnd();
            left = new OrLogical(left, right);
        }

        return left;
    }

    /// <summary>
    /// Parses logical AND expressions (&&)
    /// </summary>
    private Expression ParseLogicalAnd()
    {
        Expression left = ParseEquality();

        while (HasMoreTokens && CurrentToken.Type == Token.Types.OP_AND)
        {
            incrementIndex();
            Expression right = ParseEquality();
            left = new AndLogical(left, right);
        }

        return left;
    }

    /// <summary>
    /// Parses equality expressions (== !=)
    /// </summary>
    private Expression ParseEquality()
    {
        Expression left = ParseComparison();

        while (HasMoreTokens && IsEqualityOperator(CurrentToken.Type))
        {
            Token.Types op = CurrentToken.Type;
            incrementIndex();
            Expression right = ParseComparison();
            
            left = op switch
            {
                Token.Types.OP_REL_EQ => new EqCompExpression(left, right),
                Token.Types.OP_REL_NOT_EQ => new NotEqCompExpression(left, right),
                _ => throw new InvalidOperationException($"Unknown equality operator: {op}")
            };
        }

        return left;
    }

    /// <summary>
    /// Parses comparison expressions (< <= > >=)
    /// </summary>
    private Expression ParseComparison()
    {
        Expression left = ParseAddSubtract();

        while (HasMoreTokens && IsComparisonOperator(CurrentToken.Type))
        {
            Token.Types op = CurrentToken.Type;
            incrementIndex();
            Expression right = ParseAddSubtract();
            
            left = op switch
            {
                Token.Types.OP_REL_LESS => new LtCompExpression(left, right),
                Token.Types.OP_REL_GREATER => new GtCompExpression(left, right),
                Token.Types.OP_REL_LESS_EQ => new LteCompExpression(left, right),
                Token.Types.OP_REL_GREATER_EQ => new GteCompExpression(left, right),
                _ => throw new InvalidOperationException($"Unknown comparison operator: {op}")
            };
        }

        return left;
    }

    /// <summary>
    /// Checks if the token is an equality operator
    /// </summary>
    private bool IsEqualityOperator(Token.Types type)
    {
        return type == Token.Types.OP_REL_EQ ||
               type == Token.Types.OP_REL_NOT_EQ;
    }

    /// <summary>
    /// Checks if the token is a comparison operator
    /// </summary>
    private bool IsComparisonOperator(Token.Types type)
    {
        return type == Token.Types.OP_REL_LESS ||
               type == Token.Types.OP_REL_GREATER ||
               type == Token.Types.OP_REL_LESS_EQ ||
               type == Token.Types.OP_REL_GREATER_EQ;
    }

    /// <summary>
    /// Parses addition and subtraction expressions
    /// </summary>
    private Expression ParseAddSubtract()
    {
        Expression left = ParseMultiplyDivide();

        while (HasMoreTokens && (CurrentToken.Type == Token.Types.OP_PLUS || CurrentToken.Type == Token.Types.OP_MINUS))
        {
            Token.Types op = CurrentToken.Type;
            incrementIndex();
            Expression right = ParseMultiplyDivide();

            left = op == Token.Types.OP_PLUS
                ? new AddExpression(left, right)
                : new SubExpression(left, right);
        }

        return left;
    }

    /// <summary>
    /// Parses multiplication and division expressions
    /// </summary>
    private Expression ParseMultiplyDivide()
    {
        Expression left = ParseUnary();

        while (HasMoreTokens && (CurrentToken.Type == Token.Types.OP_MULT || CurrentToken.Type == Token.Types.OP_DIVIDE))
        {
            Token.Types op = CurrentToken.Type;
            incrementIndex();
            Expression right = ParseUnary();

            left = op == Token.Types.OP_MULT
                ? new MultExpression(left, right)
                : new DivExpression(left, right);
        }

        return left;
    }

    /// <summary>
    /// Parses unary operators (pre-increment/decrement, unary +/-, and logical NOT)
    /// </summary>
    private Expression ParseUnary()
    {
        switch (CurrentToken.Type)
        {
            case Token.Types.OP_INCR:
                incrementIndex();
                Expression preIncExpr = ParseUnary();
                return new PreIncExpression(preIncExpr);

            case Token.Types.OP_DECR:
                incrementIndex();
                Expression preDecExpr = ParseUnary();
                return new PreDecExpression(preDecExpr);

            case Token.Types.OP_PLUS:
                incrementIndex();
                Expression posExpr = ParseUnary();
                return new PrePosExpression(posExpr);

            case Token.Types.OP_MINUS:
                incrementIndex();
                Expression negExpr = ParseUnary();
                return new PreNegExpression(negExpr);

            case Token.Types.OP_NOT:
                incrementIndex();
                Expression notExpr = ParseUnary();
                return new NotLogical(notExpr);

            default:
                return ParsePostfix();
        }
    }

    /// <summary>
    /// Parses postfix operators (post-increment/decrement) and primary expressions
    /// </summary>
    private Expression ParsePostfix()
    {
        Expression expr = ParsePrimary();

        while (HasMoreTokens && (CurrentToken.Type == Token.Types.OP_INCR || CurrentToken.Type == Token.Types.OP_DECR))
        {
            Token.Types op = CurrentToken.Type;
            incrementIndex();

            expr = op == Token.Types.OP_INCR
                ? new PostIncExpression(expr)
                : new PostDecExpression(expr);
        }

        return expr;
    }

    /// <summary>
    /// Parses primary expressions (values and bracketed expressions)
    /// </summary>
    private Expression ParsePrimary()
    {
        switch (CurrentToken.Type)
        {
            case Token.Types.K_SPAWN:
                // Handle thread spawn as an expression: spawn funcCall(...)
                incrementIndex();
                Expression? spawned = (Expression?)delegateToHandler(valueHandler);
                if (spawned is FunctionCallValue funcCall)
                {
                    return new SpawnThreadNode(funcCall);
                }
                throw new SyntaxErrorException(["function call"], PrevToken);
            case Token.Types.D_PAR_OP:
                // Handle parenthesized expressions: (expression)
                incrementIndex();
                Expression parenExpr = ParseExpression();
                
                if (CurrentToken.Type != Token.Types.D_PAR_CLO)
                {
                    throw new SyntaxErrorException([")"], CurrentToken);
                }
                incrementIndex();
                return parenExpr;

            case Token.Types.D_BRAC_OP:
                // Handle array bracket expressions: [expression]
                incrementIndex();
                Expression bracketExpr = ParseExpression();
                
                if (CurrentToken.Type != Token.Types.D_BRAC_CLO)
                {
                    throw new SyntaxErrorException(["]"], CurrentToken);
                }
                incrementIndex();
                return bracketExpr;

            default:
                // Parse a value (literal, identifier, or function call)
                Expression? result = (Expression?)delegateToHandler(valueHandler);
                if (result == null)
                {
                    // Error already recorded by delegateToHandler, return a placeholder
                    return new LiteralValue(new TypeNode(DataTypes.UNKNOWN), "");
                }
                return result;
        }
    }
}

