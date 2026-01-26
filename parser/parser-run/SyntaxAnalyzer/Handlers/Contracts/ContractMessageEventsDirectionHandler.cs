using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class ContractMessageEventsDirectionHandler: Handler
{
    private readonly ContractMessageEventHandler contractMessageEventHandler;
    public ContractMessageEventsDirectionHandler(ErrorsHandler errorsHandler) : base(errorsHandler)
    {
        contractMessageEventHandler = new ContractMessageEventHandler(errorsHandler);
    }

    protected override ParseNode? verifyTokens()
    {
        expectTokenType(Token.Types.D_BRAC_OP);
        assertTokenType(Token.Types.IDENT);
        string initialRole = CurrentToken.Value;
        incrementIndex();
        expectTokenType(Token.Types.OP_RIGHT_OP);
        assertTokenType(Token.Types.IDENT);
        string targetRole = CurrentToken.Value;
        incrementIndex();
        expectTokenType(Token.Types.D_BRAC_CLO);
        var messageEventsList = new PrintableList<ContractMessageEventNode>();

        assertTokenType(Token.Types.IDENT);
        ContractMessageEventNode? messageEvent = delegateToHandler(contractMessageEventHandler) as ContractMessageEventNode;
        if (messageEvent == null) return null;
        messageEventsList.Add(messageEvent);
        while (HasMoreTokens && CurrentToken.Type == Token.Types.IDENT)
        {
            messageEvent = delegateToHandler(contractMessageEventHandler) as ContractMessageEventNode;
            if (messageEvent == null) return null;
            messageEventsList.Add(messageEvent);
        }
        return new ContractMessageEventsNode(
            new RoleNode(initialRole),
            new RoleNode(targetRole),
            messageEventsList
        );
    }
}