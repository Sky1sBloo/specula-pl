using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class ListenerOnEventHandler : Handler
{
    private readonly ListenerMessageEventHandler messageEventHandler;
    private readonly ListenerFailEventHandler failEventHandler;

    public ListenerOnEventHandler(ErrorsHandler errorsHandler) : base(errorsHandler)
    {
        messageEventHandler = new(errorsHandler);
        failEventHandler = new(errorsHandler);
    }

    protected override ParseNode? verifyTokens()
    {
        expectTokenType(Token.Types.K_ON);
        
        // Determine which handler to delegate to based on next token
        switch (CurrentToken.Type)
        {
            case Token.Types.IDENT:
                return delegateToHandler(messageEventHandler);
            case Token.Types.K_FAIL:
                return delegateToHandler(failEventHandler);
            default:
                throw new SyntaxErrorException(
                    ["IDENT", "fail"],
                    CurrentToken
                );
        }
    }
}
