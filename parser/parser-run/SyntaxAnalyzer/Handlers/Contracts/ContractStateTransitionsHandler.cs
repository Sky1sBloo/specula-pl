using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class ContractStateTransitionsHandler : Handler
{
    public ContractStateTransitionsHandler(ErrorsHandler errorsHandler) : base(errorsHandler)
    {
    }

    protected override ParseNode? verifyTokens()
    {
        var allTransitions = new PrintableList<StateTransition>();
        bool sawStateDeclaration = false;

        while (HasMoreTokens && CurrentToken.Type == Token.Types.K_STATE)
        {
            sawStateDeclaration = true;
            expectTokenType(Token.Types.K_STATE);
            assertTokenType(Token.Types.IDENT);
            string currentStateName = CurrentToken.Value;
            incrementIndex();

            while (HasMoreTokens && (CurrentToken.Type == Token.Types.OP_RIGHT_OP || 
                                      CurrentToken.Type == Token.Types.OP_BIDIR_OP))
            {
                if (CurrentToken.Type == Token.Types.OP_RIGHT_OP)
                {
                    incrementIndex();
                    assertTokenType(Token.Types.IDENT);
                    string targetStateName = CurrentToken.Value;
                    incrementIndex();
                    
                    allTransitions.Add(new StateTransitionNode(
                        new StateNode(currentStateName), 
                        new StateNode(targetStateName)
                    ));
                    currentStateName = targetStateName;
                }
                else if (CurrentToken.Type == Token.Types.OP_BIDIR_OP)
                {
                    incrementIndex();
                    assertTokenType(Token.Types.IDENT);
                    string targetStateName = CurrentToken.Value;
                    incrementIndex();
                    
                    allTransitions.Add(new StateTransitionBidirectionalNode(
                        new StateNode(currentStateName), 
                        new StateNode(targetStateName)
                    ));
                    currentStateName = targetStateName;
                }
            }

            expectTokenType(Token.Types.D_SEMICOLON);
        }

        if (!sawStateDeclaration)
        {
            return null;
        }

        return new StateTransitionsNode(allTransitions);
    }
}