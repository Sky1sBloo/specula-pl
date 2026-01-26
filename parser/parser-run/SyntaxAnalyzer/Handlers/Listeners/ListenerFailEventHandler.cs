using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class ListenerFailEventHandler : Handler
{
    private readonly BodyHandler bodyHandler;
    private readonly FuncParamsHandler failParamsHandler;

    public ListenerFailEventHandler(ErrorsHandler errorsHandler) : base(errorsHandler)
    {
        bodyHandler = new(errorsHandler, true);
        failParamsHandler = new(errorsHandler, typesOptional: true, openingToken: Token.Types.D_PAR_OP, closingToken: Token.Types.D_PAR_CLO);
    }

    protected override ParseNode? verifyTokens()
    {
        expectTokenType(Token.Types.K_FAIL);
        return parseFailEvent();
    }

    private ListenerFailEventNode? parseFailEvent()
    {
        assertTokenType(Token.Types.IDENT);
        string name = CurrentToken.Value;
        incrementIndex();

        FuncParams? parameters;

        if (HasMoreTokens && CurrentToken.Type == Token.Types.D_PAR_OP)
        {
            int savedIndex = getIndex();
            incrementIndex();
            if (HasMoreTokens && CurrentToken.Type == Token.Types.D_PAR_CLO)
            {
                incrementIndex(); 
                if (HasMoreTokens && CurrentToken.Type == Token.Types.D_PAR_OP)
                {
                    setIndex(savedIndex);
                    FuncParams? parsedParams = (FuncParams?)delegateToHandler(failParamsHandler);
                    if (parsedParams == null)
                        return null;
                    parameters = parsedParams;
                }
                else
                {
                    setIndex(savedIndex);
                    parameters = new FuncParams(new PrintableList<FuncParam>());
                }
            }
            else
            {
                setIndex(savedIndex);
                FuncParams? parsedParams = (FuncParams?)delegateToHandler(failParamsHandler);
                if (parsedParams == null)
                    return null;
                parameters = parsedParams;
            }
        }
        else
        {
            parameters = new FuncParams(new PrintableList<FuncParam>());
        }

        BodyNode? body = parseFunctionBody();
        if (body == null)
            return null;

        return new ListenerFailEventNode(name, parameters, body);
    }

    private BodyNode? parseFunctionBody()
    {
        ParseNode? bodyNode = delegateToHandler(bodyHandler);
        if (bodyNode == null)
        {
            return null;
        }
        return (BodyNode?)bodyNode;
    }
}
