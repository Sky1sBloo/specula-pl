using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

/// <summary>
/// Parses a single statement (not multiple statements like BodyHandler)
/// </summary>
public class StatementHandler : Handler
{
    private readonly IdentifierStartHandler identifierStartHandler;
    private readonly ExpressionHandler expressionHandler;
    private readonly ListenerRespondHandler? respondHandler;
    private readonly ListenerFailHandler? failHandler;
    private readonly bool isListenerContext;
    
    public StatementHandler(ErrorsHandler err, bool isListenerContext = false) : base(err)
    {
        expressionHandler = new(err);
        identifierStartHandler = new(err);
        this.isListenerContext = isListenerContext;
        
        if (isListenerContext)
        {
            respondHandler = new ListenerRespondHandler(err);
            failHandler = new ListenerFailHandler(err);
        }
    }

    protected override ParseNode? verifyTokens()
    {
        switch (CurrentToken.Type)
        {
            case Token.Types.K_AWAIT:
                {
                    // await <expression>;
                    incrementIndex();
                    Expression? awaited = (Expression?)delegateToHandler(expressionHandler);
                    if (awaited == null)
                    {
                        return null;
                    }
                    expectTokenType(Token.Types.D_SEMICOLON);
                    return new AwaitNode(awaited);
                }
            case Token.Types.K_RESPOND:
                if (isListenerContext && respondHandler != null)
                {
                    return delegateToHandler(respondHandler);
                }
                throw new SyntaxErrorException(
                    ["statement"],
                    CurrentToken);
            case Token.Types.K_FAIL:
                if (isListenerContext && failHandler != null)
                {
                    return delegateToHandler(failHandler);
                }
                throw new SyntaxErrorException(
                    ["statement"],
                    CurrentToken);
            case Token.Types.K_LET:
                {
                    ParseNode? declStmt = handleDeclarationStmt();
                    return declStmt;
                }
            case Token.Types.IDENT:
                int errorCountBefore = errorHandler.ErrorList.Count;
                ParseNode? identResult = tryHandleIdentifierStartWithResult();
                if (identResult != null)
                {
                    requireSemicolon();
                    return identResult;
                }
                
                // Clear errors from failed identifier start attempt
                while (errorHandler.ErrorList.Count > errorCountBefore)
                {
                    errorHandler.ErrorList.RemoveAt(errorHandler.ErrorList.Count - 1);
                }

                int errorsBeforeExpression = errorHandler.ErrorList.Count;

                // Try expression with error recording to get actual error
                ParseNode? exprResult = tryHandleExpressionStmtRecordingErrors();
                if (exprResult != null)
                {
                    requireSemicolon();
                    return exprResult;
                }
                
                if (errorHandler.ErrorList.Count == errorsBeforeExpression)
                {
                    throw new SyntaxErrorException(
                        ["assignment", "expression"],
                        CurrentToken);
                }
                return null;
            case Token.Types.K_IF:
                return handleIfStatement();
            case Token.Types.K_FOR:
                return handleForLoop();
            case Token.Types.K_DO:
                return handleDoWhileLoop();
            case Token.Types.K_WHILE:
                return handleWhileLoop();
            case Token.Types.K_RET:
                {
                    incrementIndex();
                    if (CurrentToken.Type == Token.Types.D_SEMICOLON)
                    {
                        return new ReturnNode(null);
                    }
                    Expression? returnValue = (Expression?)delegateToHandler(expressionHandler);
                    expectTokenType(Token.Types.D_SEMICOLON);
                    return new ReturnNode(returnValue);
                }
            default:
                ParseNode? stmtResult = tryHandleExpressionStmt();
                if (stmtResult != null)
                {
                    requireSemicolon();
                    return stmtResult;
                }
                throw new SyntaxErrorException(
                    ["statement", "expression", "declaration"],
                    CurrentToken);
        }
    }

    private void requireSemicolon()
    {
        if (!HasMoreTokens || CurrentToken.Type != Token.Types.D_SEMICOLON)
        {
            throw new SyntaxErrorException([";"], CurrentToken);
        }
        incrementIndex();
    }

    private ParseNode? handleDeclarationStmt()
    {
        return delegateToHandler(new DeclarationHandler(errorHandler));
    }

    private ParseNode? handleIfStatement()
    {
        return delegateToHandler(new ConditionalStatementHandler(errorHandler, isListenerContext));
    }

    private ParseNode? handleForLoop()
    {
        return delegateToHandler(new ForLoopHandler(errorHandler, isListenerContext));
    }

    private ParseNode? handleDoWhileLoop()
    {
        return delegateToHandler(new DoWhileLoopHandler(errorHandler, isListenerContext));
    }

    private ParseNode? handleWhileLoop()
    {
        return delegateToHandler(new WhileLoopHandler(errorHandler, isListenerContext));
    }

    private ParseNode? tryHandleIdentifierStartWithResult()
    {
        return tryDelegateToHandler(identifierStartHandler);
    }

    private ParseNode? tryHandleExpressionStmt()
    {
        try
        {
            return tryDelegateToHandler(expressionHandler);
        }
        catch (SyntaxErrorException)
        {
            return null;
        }
    }

    /// <summary>
    /// Tries to parse an expression statement, recording all errors
    /// Returns the expression if successful, null otherwise (errors will be recorded)
    /// </summary>
    private ParseNode? tryHandleExpressionStmtRecordingErrors()
    {
        try
        {
            return delegateToHandler(new ExpressionHandler(errorHandler));
        }
        catch (SyntaxErrorException)
        {
            // Expression parsing failed, errors should be recorded
            return null;
        }
    }
}
