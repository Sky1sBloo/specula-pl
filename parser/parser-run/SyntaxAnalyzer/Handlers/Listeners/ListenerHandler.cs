using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class ListenerHandler : Handler
{
    private readonly ExpressionHandler expressionHandler;
    private readonly ListenerBodyHandler listenerBodyHandler;

    public ListenerHandler(ErrorsHandler errorsHandler) : base(errorsHandler)
    {
        expressionHandler = new(errorsHandler);
        listenerBodyHandler = new(errorsHandler);
    }

    protected override ParseNode? verifyTokens()
    {
        expectTokenType(Token.Types.K_LISTENER);
        return parseListener();
    }

    private ListenerNode? parseListener()
    {
        expectTokenType(Token.Types.K_TARGET);
        expectTokenType(Token.Types.D_PAR_OP);
        
        ParseNode? targetNode = delegateToHandler(expressionHandler);
        if (targetNode == null)
            return null;
        
        ValueNode? target = (ValueNode?)targetNode;
        if (target == null) return null;

        expectTokenType(Token.Types.D_PAR_CLO);

        expectTokenType(Token.Types.K_USING);

        assertTokenType(Token.Types.IDENT);
        string contractName = CurrentToken.Value;
        incrementIndex();

        expectTokenType(Token.Types.K_AS);

        assertTokenType(Token.Types.IDENT);
        string roleName = CurrentToken.Value;
        incrementIndex();
        var role = new RoleNode(roleName);

        expectTokenType(Token.Types.D_CBRAC_OP);

        PrintableList<ListenerBody> bodyItems = new();
        while (HasMoreTokens && CurrentToken.Type != Token.Types.D_CBRAC_CLO)
        {
            ParseNode? bodyPartNode = delegateToHandler(listenerBodyHandler);
            if (bodyPartNode == null)
                break;
            
            if (bodyPartNode is ListenerBody bodyPart)
            {
                bodyItems.Add(bodyPart);
            }
            else
            {
                throw new SyntaxErrorException(["Listener Body"], CurrentToken);
            }
        }

        expectTokenType(Token.Types.D_CBRAC_CLO);

        return new ListenerNode(role, contractName, target, bodyItems);
    }
}
