using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class ListenerBodyHandler : Handler
{
    private readonly ListenerOnEventHandler onEventHandler;
    private readonly ListenerEventHandler eventHandler;
    private readonly DeclarationHandler varDeclHandler;
    private readonly FuncDefHandler funcDefHandler;

    public ListenerBodyHandler(ErrorsHandler errorsHandler) : base(errorsHandler)
    {
        onEventHandler = new(errorsHandler);
        eventHandler = new(errorsHandler);
        varDeclHandler = new(errorsHandler);
        funcDefHandler = new(errorsHandler);
    }

    protected override ParseNode? verifyTokens()
    {
        // Determine which type of body part to parse based on current token
        switch (CurrentToken.Type)
        {
            case Token.Types.K_ON:
                return delegateToHandler(onEventHandler);
            case Token.Types.K_BEFORE:
            case Token.Types.K_AFTER:
                return delegateToHandler(eventHandler);
            case Token.Types.K_LET:
                return delegateToHandler(varDeclHandler);
            case Token.Types.K_FN:
            case Token.Types.K_THREAD:
                return delegateToHandler(funcDefHandler);
            default:
                throw new SyntaxErrorException(
                    ["on", "before", "after", "let", "func", "thread"],
                    CurrentToken
                );
        }
    }
}
