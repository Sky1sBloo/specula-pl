using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;


public class InitStateHandler: Handler
{
    public InitStateHandler(ErrorsHandler errorsHandler) : base(errorsHandler)
    {
    }

    protected override ParseNode? verifyTokens()
    {
        expectTokenType(Token.Types.K_INIT_STATE);
        assertTokenType(Token.Types.IDENT);
        string stateName = CurrentToken.Value;
        incrementIndex();
        expectTokenType(Token.Types.D_SEMICOLON);

        return new InitStateNode(new StateNode(stateName));
    }
}