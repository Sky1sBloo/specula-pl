using System.Reflection;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class SyntaxAnalyzerRoot
{
    public readonly ErrorsHandler ErrorHandler;
    private readonly FuncDefHandler funcDefHandler;
    private readonly DeclarationHandler declarationHandler;
    private readonly ImportsHandler importsHandler;
    private readonly ExportHandler exportHandler;
    private readonly StructHandler structHandler;
    private readonly InterfaceHandler interfaceHandler;
    private readonly ImplHandler implHandler;
    private readonly ContractHandler contractHandler;
    private readonly ListenerHandler listenerHandler;

    private PrintableList<RootStatement> statements = new();
    private List<Token> tokens = [];
    private int i = 0;

    public SyntaxAnalyzerRoot()
    {
        ErrorHandler = new();
        funcDefHandler = new FuncDefHandler(ErrorHandler);
        declarationHandler = new DeclarationHandler(ErrorHandler);
        importsHandler = new ImportsHandler(ErrorHandler);
        exportHandler = new ExportHandler(ErrorHandler);
        structHandler = new StructHandler(ErrorHandler);
        interfaceHandler = new InterfaceHandler(ErrorHandler);
        implHandler = new ImplHandler(ErrorHandler);
        contractHandler = new ContractHandler(ErrorHandler);
        listenerHandler = new ListenerHandler(ErrorHandler);
    }

    public SyntaxAnalyzerRoot(ErrorsHandler errors)
    {
        ErrorHandler = errors;
        funcDefHandler = new FuncDefHandler(ErrorHandler);
        declarationHandler = new DeclarationHandler(ErrorHandler);
        importsHandler = new ImportsHandler(ErrorHandler);
        exportHandler = new ExportHandler(ErrorHandler);
        structHandler = new StructHandler(ErrorHandler);
        interfaceHandler = new InterfaceHandler(ErrorHandler);
        implHandler = new ImplHandler(ErrorHandler);
        contractHandler = new ContractHandler(ErrorHandler);
        listenerHandler = new ListenerHandler(ErrorHandler);
    }

    public ParseNode? ReadTokens(List<Token> tokens)
    {
        this.tokens = tokens;
        this.i = 0;
        statements = new PrintableList<RootStatement>();

        while (i < tokens.Count)
        {
            RootStatement? stmt = parseRootStatement();
            if (stmt != null)
            {
                statements.Add(stmt);
            }
        }

        return new RootNode(statements);
    }

    private RootStatement? parseRootStatement()
    {
        if (i >= tokens.Count)
            return null;

        Token currentToken = tokens[i];

        switch (currentToken.Type)
        {
            case Token.Types.K_FN:
            case Token.Types.K_THREAD:
                return handleFuncDef();
            case Token.Types.K_LET:
                return handleDeclaration();
            case Token.Types.K_IMPORT:
                return handleImport();
            case Token.Types.D_BRAC_OP:
                return handleExport();
            case Token.Types.K_STRUCT:
                return handleStruct();
            case Token.Types.K_INTERFACE:
                return handleInterface();
            case Token.Types.K_IMPL:
                return handleImpl();
            case Token.Types.K_CONTRACT:
                return handleContract();
            case Token.Types.K_LISTENER:
                return (RootStatement?)delegateToHandler(listenerHandler);
            default:
                try
                {
                    throw new SyntaxErrorException(
                        ["fn", "thread", "let", "import", "[", "struct", "interface", "impl", "contract", "listener"],
                        currentToken);
                }
                catch (SyntaxErrorException ex)
                {
                    ErrorHandler.AddError(ex);
                    while (i < tokens.Count && (tokens[i].Type != Token.Types.D_SEMICOLON || tokens[i].Type != Token.Types.D_BRAC_CLO))
                    {
                        i++;
                    }
                    if (i < tokens.Count && (tokens[i].Type == Token.Types.D_SEMICOLON || tokens[i].Type == Token.Types.D_BRAC_CLO))
                    {
                        i++;
                    }
                    return null;
                }
        }
    }

    private RootStatement? handleFuncDef()
    {
        return (RootStatement?)delegateToHandler(funcDefHandler);
    }

    private RootStatement? handleDeclaration()
    {
        RootStatement? stmt = (RootStatement?)delegateToHandler(declarationHandler);
        // Declaration handler stops at semicolon, consume it
        if (stmt != null && i < tokens.Count && tokens[i].Type == Token.Types.D_SEMICOLON)
        {
            i++;
        }
        return stmt;
    }

    private RootStatement? handleImport()
    {
        return (RootStatement?)delegateToHandler(importsHandler);
    }
    private RootStatement? handleExport()
    {
        return (RootStatement?)delegateToHandler(exportHandler);
    }

    private StructDefNode? handleStruct()
    {
        return (StructDefNode?)delegateToHandler(structHandler);
    }

    private InterfaceDefNode? handleInterface()
    {
        return (InterfaceDefNode?)delegateToHandler(interfaceHandler);
    }

    private ImplDefNode? handleImpl()
    {
        return (ImplDefNode?)delegateToHandler(implHandler);
    }

    private ContractNode? handleContract()
    {
        return (ContractNode?)delegateToHandler(contractHandler);
    }

    private ParseNode? delegateToHandler(Handler handler)
    {
        try
        {
            HandlerOutput output = handler.HandleToken(tokens, i);
            i = output.endIndex;
            return output.node;
        }
        catch (SyntaxErrorException ex)
        {
            ErrorHandler.AddError(ex);
            while (i < tokens.Count && tokens[i].Type != Token.Types.D_SEMICOLON)
            {
                i++;
            }
            if (i < tokens.Count && tokens[i].Type == Token.Types.D_SEMICOLON)
            {
                i++;
            }
        }
        return null;
    }

}
