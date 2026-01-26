using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

/// <summary>
/// Contains conditions 
/// </summary>
public class ConditionalStatementHandler : Handler
{
    private readonly ExpressionHandler expressionHandler;
    private readonly BodyHandler bodyHandler;

    public ConditionalStatementHandler(ErrorsHandler errors, bool isListenerContext = false) : base(errors)
    {
        expressionHandler = new(errors);
        bodyHandler = new(errors, isListenerContext);
    }

    protected override ParseNode? verifyTokens()
    {
        if (CurrentToken.Type != Token.Types.K_IF)
        {
            throw new SyntaxErrorException(["if"], CurrentToken);
        }
        incrementIndex();
        IfStatementNode ifStatement = ParseIfStatement();
        PrintableList<ElseIfStatementNode> elseIfStatements = new();
        ElseStatement? elseStatement = null;
        while (HasMoreTokens)
        {
            if (CurrentToken.Type == Token.Types.K_ELSE)
            {
                incrementIndex();
                if (CurrentToken.Type == Token.Types.K_IF)
                {
                    incrementIndex();
                    elseIfStatements.Add(parseElseIfStatement());
                }
                else
                {
                    elseStatement = parseElseStatement();
                    break;
                }
            }
            else
            {
                break;
            }
        }
        return new ConditionalStatement(ifStatement, elseIfStatements, elseStatement);
    }

    // already consumed the 'if' token
    private IfStatementNode ParseIfStatement()
    {
        return (IfStatementNode)parseConditionBody(true);
    }
    // already consumed the 'else' and 'if' tokens
    private ElseIfStatementNode parseElseIfStatement()
    {
        return (ElseIfStatementNode)parseConditionBody(false);
    }
    private IfNode parseConditionBody(bool buildIfStatement)
    {

        if (CurrentToken.Type != Token.Types.D_PAR_OP)
        {
            SyntaxErrorException ex = new SyntaxErrorException(["("], CurrentToken);
            errorHandler.AddError(ex);
            throw ex;
        }

        ParseNode? conditionNode = delegateToHandler(expressionHandler);
        if (conditionNode is null)
        {
            throw new SyntaxErrorException(["Expression"], CurrentToken);
        }
        Expression conditionExpr = (Expression)conditionNode;

        ParseNode? bodyNode = delegateToHandler(bodyHandler);
        if (bodyNode is null)
        {
            throw new SyntaxErrorException(["Body"], CurrentToken);
        }
        BodyNode body = (BodyNode)bodyNode;
        if (buildIfStatement)
        {
            return new IfStatementNode(conditionExpr, body);
        }
        else
        {
            return new ElseIfStatementNode(conditionExpr, body);
        }
    }
    private ElseStatement parseElseStatement()
    {
        ParseNode? bodyNode = delegateToHandler(bodyHandler);
        if (bodyNode is null)
        {
            throw new SyntaxErrorException(["Body"], CurrentToken);
        }
        return new ElseStatement((BodyNode)bodyNode);
    }
}