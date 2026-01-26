using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

/// <summary>
/// Handles listener fail statement: fail low_battery; or fail low_battery(true, a);
/// </summary>
public class ListenerFailHandler : Handler
{
    public ListenerFailHandler(ErrorsHandler err) : base(err)
    {
    }

    protected override ParseNode? verifyTokens()
    {
        // expect 'fail' keyword
        assertTokenType(Token.Types.K_FAIL);
        incrementIndex();
        
        // expect identifier (fail name)
        assertTokenType(Token.Types.IDENT);
        string failName = CurrentToken.Value;
        incrementIndex();
        
        PrintableList<Expression> arguments;
        
        // Check if there are arguments
        if (HasMoreTokens && CurrentToken.Type == Token.Types.D_PAR_OP)
        {
            incrementIndex();
            arguments = parseArguments();
        }
        else
        {
            arguments = new PrintableList<Expression>();
        }
        
        // expect semicolon
        assertTokenType(Token.Types.D_SEMICOLON);
        incrementIndex();
        
        return new ListenerFail(failName, arguments);
    }

    private PrintableList<Expression> parseArguments()
    {
        var arguments = new PrintableList<Expression>();

        // For empty argument list
        if (HasMoreTokens && CurrentToken.Type == Token.Types.D_PAR_CLO)
        {
            incrementIndex();
            return arguments;
        }

        while (HasMoreTokens && CurrentToken.Type != Token.Types.D_PAR_CLO)
        {
            ExpressionHandler exprHandler = new ExpressionHandler(errorHandler);
            ParseNode? argExpr = delegateToHandler(exprHandler);
            
            if (argExpr is Expression expr)
            {
                arguments.Add(expr);
            }
            else
            {
                throw new SyntaxErrorException(["expression"], CurrentToken);
            }

            // Handle comma between arguments or closing paren
            if (HasMoreTokens && CurrentToken.Type == Token.Types.COMMA)
            {
                incrementIndex();
            }
            else if (HasMoreTokens && CurrentToken.Type != Token.Types.D_PAR_CLO)
            {
                throw new SyntaxErrorException([",", ")"], CurrentToken);
            }
        }

        if (!HasMoreTokens || CurrentToken.Type != Token.Types.D_PAR_CLO)
        {
            Token missing = new()
            {
                Type = Token.Types.UNKNOWN,
                Line = getIndex(),
                CharStart = 0,
                CharEnd = 0
            };
            throw new SyntaxErrorException([")"], missing);
        }

        incrementIndex();
        return arguments;
    }
}
