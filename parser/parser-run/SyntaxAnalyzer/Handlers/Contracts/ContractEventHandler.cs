using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class ContractEventHandler : Handler
{
    private readonly FuncParamsHandler paramsHandler;
    public ContractEventHandler(ErrorsHandler errorsHandler) : base(errorsHandler)
    {
        paramsHandler = new(errorsHandler, typesOptional: false);
    }

    protected override ParseNode? verifyTokens()
    {
        switch (CurrentToken.Type)
        {
            case Token.Types.K_AUTO_RESET:
                return parseAutoResetEvent();
            case Token.Types.K_AUTO_MOVE:
                return parseAutoMoveEvent();
            case Token.Types.K_FAIL:
                return parseFailEvent();
            default:
                throw new SyntaxErrorException(
                    ["'auto reset'", "'auto move'", "'fail'"], 
                    CurrentToken
                );
        }
    }

    private ContractAutoResetEventNode parseAutoResetEvent()
    {
        expectTokenType(Token.Types.K_AUTO_RESET);
        expectTokenType(Token.Types.K_AFTER);
        
        var statesList = new PrintableList<StateNode>();
        assertTokenType(Token.Types.IDENT);
        statesList.Add(new StateNode(CurrentToken.Value));
        incrementIndex();
        
        while (HasMoreTokens && CurrentToken.Type == Token.Types.OP_OR)
        {
            incrementIndex();
            assertTokenType(Token.Types.IDENT);
            statesList.Add(new StateNode(CurrentToken.Value));
            incrementIndex();
        }
        
        expectTokenType(Token.Types.D_SEMICOLON);
        return new ContractAutoResetEventNode(statesList);
    }

    private ContractAutoMoveEventNode parseAutoMoveEvent()
    {
        expectTokenType(Token.Types.K_AUTO_MOVE);
        expectTokenType(Token.Types.K_AFTER);
        
        var statesList = new PrintableList<StateNode>();
        assertTokenType(Token.Types.IDENT);
        statesList.Add(new StateNode(CurrentToken.Value));
        incrementIndex();
        
        while (HasMoreTokens && CurrentToken.Type == Token.Types.OP_OR)
        {
            incrementIndex();
            assertTokenType(Token.Types.IDENT);
            statesList.Add(new StateNode(CurrentToken.Value));
            incrementIndex();
        }
        
        expectTokenType(Token.Types.K_TO);
        assertTokenType(Token.Types.IDENT);
        string targetState = CurrentToken.Value;
        incrementIndex();
        expectTokenType(Token.Types.D_SEMICOLON);
        return new ContractAutoMoveEventNode(statesList, new StateNode(targetState));
    }

    private ContractFailEventNode parseFailEvent()
    {
        expectTokenType(Token.Types.K_FAIL);
        assertTokenType(Token.Types.IDENT);
        string identifier = CurrentToken.Value;
        incrementIndex();
        
        if (CurrentToken.Type == Token.Types.D_SEMICOLON)
        {
            incrementIndex();
            return new ContractFailEventNode(identifier, new FuncParams(new PrintableList<FuncParam>()));
        }
        
        FuncParams? parameters = (FuncParams?)delegateToHandler(paramsHandler);
        if (parameters == null) return null!;
        
        expectTokenType(Token.Types.D_SEMICOLON);
        return new ContractFailEventNode(identifier, parameters);
    }
}