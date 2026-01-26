using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

/// <summary>
/// Contains conditions 
/// </summary>
public class ForLoopHandler : Handler
{
    private readonly ExpressionHandler expressionHandler;
    private readonly BodyHandler bodyHandler;
    private readonly DeclarationHandler declarationHandler;
    private readonly VarAssignHandler varAssignHandler;

    public ForLoopHandler(ErrorsHandler errors, bool isListenerContext = false) : base(errors)
    {
        expressionHandler = new(errors);
        bodyHandler = new(errors, isListenerContext);
        declarationHandler = new(errors);
        varAssignHandler = new(errors);
    }

    protected override ParseNode? verifyTokens()
    {
        ForInit? forInit = null;
        Expression? expression;
        Assignment? assignment;
        BodyNode? body;
        if (CurrentToken.Type != Token.Types.K_FOR)
        {
            throw new SyntaxErrorException(["for"], CurrentToken);
        }
        incrementIndex();
        if (CurrentToken.Type != Token.Types.D_PAR_OP)
        {
            throw new SyntaxErrorException(["("], CurrentToken);
        }
        incrementIndex();

        switch (CurrentToken.Type)
        {
            case Token.Types.K_LET:
                forInit = handleDeclaration();
                break;
            case Token.Types.IDENT:
                forInit = handleAssignment();
                break;
        }
        bool initConsumedSemicolon = PrevToken?.Type == Token.Types.D_SEMICOLON;

        if (CurrentToken.Type == Token.Types.D_SEMICOLON)
        {
            expectTokenType(Token.Types.D_SEMICOLON);
        }
        else if (!initConsumedSemicolon)
        {
            throw new SyntaxErrorException([";"], CurrentToken);
        }

        expression = handleExpression();
        expectTokenType(Token.Types.D_SEMICOLON);

        assignment = handleAssignment();
        expectTokenType(Token.Types.D_PAR_CLO);
        body = handleBody();

        if (forInit is null || expression is null || assignment is null || body is null)
        {
            return null;
        }

        return new ForLoop(forInit, expression, assignment, body);
    }

    private DeclarationStatementNode? handleDeclaration()
    {
        ParseNode? node = delegateToHandler(declarationHandler);
        if (node == null)
            return null;
        return (DeclarationStatementNode)node;
    }

    private Assignment? handleAssignment()
    {
        ParseNode? node = delegateToHandler(varAssignHandler);
        if (node == null)
            return null;
        return (Assignment)node;
    }

    private Expression? handleExpression()
    {
        ParseNode? node = delegateToHandler(expressionHandler);
        if (node == null)
            return null;
        return (Expression)node;
    }

    private BodyNode? handleBody()
    {
        ParseNode? node = delegateToHandler(bodyHandler);
        if (node == null)
            return null;
        return (BodyNode)node;
    }
}
