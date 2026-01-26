using System.Diagnostics.Contracts;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class ContractHandler : Handler
{
    private readonly InitStateHandler initStateHandler;
    private readonly ContractRoleHandler contractRoleHandler;
    private readonly ContractStateTransitionsHandler contractStateTransitionsHandler;
    private readonly ContractMessageEventsDirectionHandler contractMessageEventHandler;
    private readonly ContractEventHandler contractEventHandler;
    public ContractHandler(ErrorsHandler errorsHandler) : base(errorsHandler)
    {
        initStateHandler = new InitStateHandler(errorsHandler);
        contractRoleHandler = new ContractRoleHandler(errorsHandler);
        contractStateTransitionsHandler = new ContractStateTransitionsHandler(errorsHandler);
        contractMessageEventHandler = new ContractMessageEventsDirectionHandler(errorsHandler);
        contractEventHandler = new ContractEventHandler(errorsHandler);
    }

    protected override ParseNode? verifyTokens()
    {
        expectTokenType(Token.Types.K_CONTRACT);
        assertTokenType(Token.Types.IDENT);
        string contractName = CurrentToken.Value;
        incrementIndex();
        // Consume opening contract brace so child handlers start at the correct token
        expectTokenType(Token.Types.D_CBRAC_OP);
        InitStateNode? initStateNode = delegateToHandler(initStateHandler) as InitStateNode;
        if (initStateNode == null) return null;
        RolesNode? rolesNode = delegateToHandler(contractRoleHandler) as RolesNode;
        if (rolesNode == null) return null;
        var stateTransitionNodes = new PrintableList<StateTransitionsNode>();
        while (HasMoreTokens && CurrentToken.Type == Token.Types.K_STATE)
        {
            var stateTransitionsNode = delegateToHandler(contractStateTransitionsHandler) as StateTransitionsNode;
            if (stateTransitionsNode == null) return null;
            stateTransitionNodes.Add(stateTransitionsNode);
        }
        // to catch orphaned message events without a direction header
        if (HasMoreTokens && CurrentToken.Type == Token.Types.IDENT)
        {
            errorHandler.AddError("Message event missing direction header [from -> to]");
            return null;
        }
        var messageEventsList = new PrintableList<ContractMessageEventsNode>();

        while (HasMoreTokens && CurrentToken.Type == Token.Types.D_BRAC_OP)
        {
            var messageEventsNode = delegateToHandler(contractMessageEventHandler) as ContractMessageEventsNode;
            if (messageEventsNode == null) return null;
            messageEventsList.Add(messageEventsNode);
        }
        var eventsList = new PrintableList<ContractEventNode>();
        while (HasMoreTokens && (CurrentToken.Type == Token.Types.K_AUTO_RESET ||
                                 CurrentToken.Type == Token.Types.K_AUTO_MOVE ||
                                 CurrentToken.Type == Token.Types.K_FAIL))
        {
            var eventNode = delegateToHandler(contractEventHandler) as ContractEventNode;
            if (eventNode == null) return null;
            eventsList.Add(eventNode);
        }
        expectTokenType(Token.Types.D_CBRAC_CLO);
        return new ContractNode(
            contractName,
            initStateNode,
            rolesNode,
            stateTransitionNodes,
            messageEventsList,
            eventsList
        );
    }
}