using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;



public class ImplHandler: Handler
{
    private readonly FuncDefHandler funcDefHandler;
    
    public ImplHandler(ErrorsHandler errors) : base(errors)
    {
        funcDefHandler = new FuncDefHandler(errors);
    }

    protected override ParseNode? verifyTokens()
    {
        expectTokenType(Token.Types.K_IMPL);
        assertTokenType(Token.Types.IDENT);
        string interfaceName = CurrentToken.Value;
        incrementIndex();
        expectTokenType(Token.Types.K_USING);
        assertTokenType(Token.Types.IDENT);
        string structName = CurrentToken.Value;
        incrementIndex();
        expectTokenType(Token.Types.D_CBRAC_OP);
        PrintableList<FuncDef> methods = new();
        while (HasMoreTokens && CurrentToken.Type != Token.Types.D_CBRAC_CLO)
        {
            // Delegate to FuncDefHandler to parse each function
            FuncDef? funcDef = (FuncDef?)delegateToHandler(funcDefHandler);
            if (funcDef == null)
            {
                return null;
            }
            methods.Add(funcDef);
        }
        if (!HasMoreTokens)
            throw new SyntaxErrorException(["}" ], new Token { Type = Token.Types.UNKNOWN });
        expectTokenType(Token.Types.D_CBRAC_CLO);
        return new ImplDefNode(structName, interfaceName, methods);
    }
}