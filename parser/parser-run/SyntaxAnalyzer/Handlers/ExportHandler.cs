using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class ExportHandler : Handler
{
    private readonly FuncDefHandler funcDefHandler;
    private readonly DeclarationHandler declarationHandler;
    private readonly StructHandler structHandler;
    private readonly InterfaceHandler interfaceHandler;
    private readonly ImplHandler implHandler;
    private readonly ContractHandler contractHandler;
    private readonly ListenerHandler listenerHandler;

    public ExportHandler(ErrorsHandler errors) : base(errors)
    {
        funcDefHandler = new FuncDefHandler(errors);
        declarationHandler = new DeclarationHandler(errors);
        structHandler = new StructHandler(errors);
        interfaceHandler = new InterfaceHandler(errors);
        implHandler = new ImplHandler(errors);
        contractHandler = new ContractHandler(errors);
        listenerHandler = new ListenerHandler(errors);
    }

    protected override ParseNode? verifyTokens()
    {
        if (CurrentToken.Type != Token.Types.D_BRAC_OP)
        {
            throw new SyntaxErrorException(["["], CurrentToken);
        }
        incrementIndex();

        bool isDefault = false;
        switch (CurrentToken.Type)
        {
            case Token.Types.K_EXPORT:
                incrementIndex();
                break;
            case Token.Types.K_EXPORT_DEFAULT:
                isDefault = true;
                incrementIndex();
                break;
            default:
                throw new SyntaxErrorException(["export", "export default"], CurrentToken);
        }

        if (CurrentToken.Type != Token.Types.D_BRAC_CLO)
        {
            throw new SyntaxErrorException(["]"], CurrentToken);
        }
        incrementIndex();

        // Parse the root statement that is being exported
        RootStatement? stmt = parseExportedRootStatement();
        if (stmt == null) return null;

        if (isDefault)
        {
            return new ExportDefaultNode(stmt);
        }
        else
        {
            return new ExportNode(stmt);
        }
    }

    private RootStatement? parseExportedRootStatement()
    {
        if (!HasMoreTokens)
        {
            throw new SyntaxErrorException(["statement", "declaration", "function"], CurrentToken);
        }

        Token currentToken = CurrentToken;
        switch (currentToken.Type)
        {
            case Token.Types.K_FN:
            case Token.Types.K_THREAD:
                return (RootStatement?)delegateToHandler(funcDefHandler);
            case Token.Types.K_LET:
                RootStatement? decl = (RootStatement?)delegateToHandler(declarationHandler);
                // Declaration handler stops at semicolon, consume it
                if (decl != null && HasMoreTokens && CurrentToken.Type == Token.Types.D_SEMICOLON)
                {
                    incrementIndex();
                }
                return decl;
            case Token.Types.K_STRUCT:
                {
                    ParseNode? node = delegateToHandler(structHandler);
                    return (RootStatement?)node;
                }
            case Token.Types.K_INTERFACE:
                {
                    ParseNode? node = delegateToHandler(interfaceHandler);
                    return (RootStatement?)node;
                }
            case Token.Types.K_IMPL:
                {
                    ParseNode? node = delegateToHandler(implHandler);
                    return (RootStatement?)node;
                }
            case Token.Types.K_CONTRACT:
                {
                    ParseNode? node = delegateToHandler(contractHandler);
                    return (RootStatement?)node;
                }
            case Token.Types.K_LISTENER:
                {
                    ParseNode? node = delegateToHandler(listenerHandler);
                    return (RootStatement?)node;
                }
            default:
                throw new SyntaxErrorException(
                    ["fn", "thread", "let", "struct"],
                    currentToken);
        }
    }
}