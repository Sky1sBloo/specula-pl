using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

/// <summary>
/// Contains everything that is within {} with expressions
/// </summary>
public class BodyHandler : Handler
{
    private PrintableList<Statement> statements = new();
    private readonly StatementHandler statementHandler;
    
    public BodyHandler(ErrorsHandler err, bool isListenerContext = false) : base(err)
    {
        statementHandler = new StatementHandler(err, isListenerContext);
    }

    protected override ParseNode? verifyTokens()
    {
        // reset statements for each new body parse to avoid leaking previous state
        statements = new PrintableList<Statement>();
        expectTokenType(Token.Types.D_CBRAC_OP);
        
        int initialErrorCount = errorHandler.ErrorList.Count;
        
        while (true)
        {
            if (!HasMoreTokens)
            {
                Token missing = new()
                {
                    Type = Token.Types.UNKNOWN,
                    Line = getIndex(),
                    CharStart = 0,
                    CharEnd = 0
                };
                throw new SyntaxErrorException(["}"], missing);
            }

            if (CurrentToken.Type == Token.Types.D_CBRAC_CLO)
            {
                incrementIndex(); 
                break;
            }

            // Delegate to StatementHandler to parse a single statement
            ParseNode? stmt = delegateToHandler(statementHandler);
            if (stmt != null)
            {
                statements.Add((Statement)stmt);
            }
        }
        
        if (errorHandler.ErrorList.Count > initialErrorCount)
        {
            return null;
        }
        
        return new BodyNode(statements);
    }
}
