using System.Reflection.Metadata;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class FuncDefHandler : Handler
{
    private readonly FuncShapeHandler funcShapeHandler;
    public FuncDefHandler(ErrorsHandler errors) : base(errors)
    {
        funcShapeHandler = new(errors);
    }

    protected override ParseNode? verifyTokens()
    {
        switch (CurrentToken.Type)
        {
            case Token.Types.K_FN:
                return handleFunctionDefinition();
            case Token.Types.K_THREAD:
                return handleThreadDefinition();
            default:
                throw new SyntaxErrorException(["fn", "thread"], CurrentToken);
        }
    }

    private FuncDefNode? handleFunctionDefinition()
    {
        incrementIndex();
        bool isAsync = false;

        if (CurrentToken.Type == Token.Types.K_ASYNC)
        {
            isAsync = true;
            incrementIndex();
        }
        assertTokenType(Token.Types.IDENT);
        string funcName = CurrentToken.Value;
        incrementIndex();
        FuncShapeNode? funcShape = (FuncShapeNode?)delegateToHandler(funcShapeHandler);
        if (funcShape == null) return null;

        return new FuncDefNode(funcName, isAsync, funcShape);
    }

    private ThreadDefNode? handleThreadDefinition()
    {
        incrementIndex();
        assertTokenType(Token.Types.IDENT);
        string funcName = CurrentToken.Value;
        Token identToken = CurrentToken;
        incrementIndex();
        FuncShapeNode? funcShape = (FuncShapeNode?)delegateToHandler(funcShapeHandler);
        if (funcShape == null) return null;
        if (funcShape.ReturnType.DataType.DataType != DataTypes.VOID)
        {
            throw new SyntaxErrorException(
                ["Thread return type must be VOID"],
                identToken
            );
        }

        return new ThreadDefNode(funcName, funcShape);
    }

}