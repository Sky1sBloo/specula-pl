using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class DoWhileLoopHandler : Handler
{
    private readonly BodyHandler bodyHandler;
    private readonly ExpressionHandler expressionHandler;
    public DoWhileLoopHandler(ErrorsHandler errors, bool isListenerContext = false) : base(errors)
    {
        bodyHandler = new(errors, isListenerContext);
        expressionHandler = new(errors);
    }

    protected override ParseNode? verifyTokens()
    {
        expectTokenType(Token.Types.K_DO);
        BodyNode? body = handleBody();
        if (body == null)
        {
            return null;
        }
        expectTokenType(Token.Types.K_WHILE);

        Expression? condition = handleCondition();
        if (condition == null)
        {
            return null;
        }
        expectTokenType(Token.Types.D_SEMICOLON);

        return new DoWhileLoop(body, condition);
    }

    private Expression? handleCondition()
    {
        expectTokenType(Token.Types.D_PAR_OP);
        Expression? condition = (Expression?)delegateToHandler(expressionHandler);
        expectTokenType(Token.Types.D_PAR_CLO);
        return condition;
    }

    private BodyNode? handleBody()
    {
        BodyNode? body = (BodyNode?)delegateToHandler(bodyHandler);
        return body!;
    }
}