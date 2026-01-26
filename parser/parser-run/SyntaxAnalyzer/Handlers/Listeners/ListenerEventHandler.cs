using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

/// <summary>
/// Handles event handler (not fail event)
/// </summary>
public class ListenerEventHandler : Handler
{
    private readonly BodyHandler bodyHandler;
    public ListenerEventHandler(ErrorsHandler err) : base(err)
    {
        bodyHandler = new BodyHandler(err, isListenerContext: true);
    }

    protected override ParseNode? verifyTokens()
    {
        ListenerEventType eventType = CurrentToken.Type switch
        {
            Token.Types.K_BEFORE => ListenerEventType.BEFORE,
            Token.Types.K_AFTER => ListenerEventType.AFTER,
            _ => throw new SyntaxErrorException(
                ["'before'", "'after'"],
                CurrentToken
            )
        };
        incrementIndex();
        assertTokenType(Token.Types.IDENT);
        string messageEventName = CurrentToken.Value;
        incrementIndex();
        BodyNode? body = delegateToHandler(bodyHandler) as BodyNode;
        if (body == null) return null;
        return new ListenerEventNode(
            messageEventName,
            eventType,
            body
            );
    }
}