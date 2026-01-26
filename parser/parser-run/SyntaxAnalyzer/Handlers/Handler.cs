using SpeculaSyntaxAnalyzer.ParseTree;
namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public struct HandlerOutput
{
    public int endIndex;
    public ParseNode? node;
}

public abstract class Handler
{
    private int i;
    protected readonly ErrorsHandler errorHandler;
    private List<Token> tokens = new();
    protected Token? PrevToken
    {
        get
        {
            if (i == 0)
            {
                return null;
            }
            return tokens[i - 1];
        }
    }

    protected Token CurrentToken
    {
        get
        {
            if (i < 0 || i >= tokens.Count)
            {
                throw new SyntaxErrorException(PrevToken);
            }
            return tokens.ElementAt(i);
        }
    }
    protected bool HasMoreTokens => i < tokens.Count;

    public Handler(ErrorsHandler errorHandler)
    {
        this.errorHandler = errorHandler;
    }

    /// <summary>
    /// Provides the handler to handle the given token
    /// </summary>
    ///
    /// <returns> The index where it ends
    public HandlerOutput HandleToken(List<Token> tokens, int iStart)
    {
        this.tokens = tokens;
        skipToIndex(iStart);
        HandlerOutput output;
        output.node = verifyTokens();
        output.endIndex = getIndex();
        return output;
    }


    /// <summary>
    /// Verifies the token list if it follows the definition
    /// </summary>
    protected abstract ParseNode? verifyTokens();

    /// <summary>
    /// Checks if the current token matches the expected type and throws if not.
    /// Also increments the index past the matched token.
    /// </summary>
    protected void expectTokenType(Token.Types expectedType)
    {
        if (!HasMoreTokens)
        {
            throw new SyntaxErrorException([TokenTypeToString(expectedType)], PrevToken);
        }

        if (CurrentToken.Type != expectedType)
        {
            throw new SyntaxErrorException([TokenTypeToString(expectedType)], CurrentToken);
        }
        incrementIndex();
    }

    /// <summary>
    /// Checks if the current token matches one of the expected types and throws if not.
    /// Also increments the index past the matched token.
    /// </summary>
    protected void expectTokenType(params Token.Types[] expectedTypes)
    {
        if (!HasMoreTokens)
        {
            var typeNames = expectedTypes.Select(TokenTypeToString).ToList();
            string expected = string.Join(", ", typeNames);
            throw new SyntaxErrorException($"Expected token: {expected}, but reached end of file");
        }

        bool found = false;
        foreach (var type in expectedTypes)
        {
            if (CurrentToken.Type == type)
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            var typeNames = expectedTypes.Select(TokenTypeToString).ToList();
            throw new SyntaxErrorException(typeNames, CurrentToken);
        }
        incrementIndex();
    }

    /// <summary>
    /// Checks if the current token matches the expected type and throws if not.
    /// Does NOT increment the index.
    /// </summary>
    protected void assertTokenType(Token.Types expectedType)
    {
        if (!HasMoreTokens)
        {
            throw new SyntaxErrorException($"Expected token: {TokenTypeToString(expectedType)}, but reached end of file");
        }

        if (CurrentToken.Type != expectedType)
        {
            throw new SyntaxErrorException([TokenTypeToString(expectedType)], CurrentToken);
        }
    }

    /// <summary>
    /// Checks if the current token matches one of the expected types and throws if not.
    /// Does NOT increment the index.
    /// </summary>
    protected void assertTokenType(params Token.Types[] expectedTypes)
    {
        if (!HasMoreTokens)
        {
            var typeNames = expectedTypes.Select(TokenTypeToString).ToList();
            string expected = string.Join(", ", typeNames);
            throw new SyntaxErrorException($"Expected token: {expected}, but reached end of file");
        }

        bool found = false;
        foreach (var type in expectedTypes)
        {
            if (CurrentToken.Type == type)
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            var typeNames = expectedTypes.Select(TokenTypeToString).ToList();
            throw new SyntaxErrorException(typeNames, CurrentToken);
        }
    }

    private static string TokenTypeToString(Token.Types type)
    {
        return type switch
        {
            Token.Types.K_FN => "fn",
            Token.Types.K_ASYNC => "async",
            Token.Types.K_LET => "let",
            Token.Types.K_IF => "if",
            Token.Types.K_ELSE => "else",
            Token.Types.K_FOR => "for",
            Token.Types.K_WHILE => "while",
            Token.Types.K_DO => "do",
            Token.Types.K_TYPE => "type",
            Token.Types.K_MOVE => "move",
            Token.Types.K_SHARE => "share",
            Token.Types.K_REF => "ref",
            Token.Types.K_STRUCT => "struct",
            Token.Types.K_VIEW => "view",
            Token.Types.K_MUT => "mut",
            Token.Types.K_CONST => "const",
            Token.Types.K_OWN => "own",
            Token.Types.D_PAR_OP => "(",
            Token.Types.D_PAR_CLO => ")",
            Token.Types.D_CBRAC_OP => "{",
            Token.Types.D_CBRAC_CLO => "}",
            Token.Types.D_BRAC_OP => "[",
            Token.Types.D_BRAC_CLO => "]",
            Token.Types.D_COLON => ":",
            Token.Types.D_SEMICOLON => ";",
            Token.Types.COMMA => ",",
            Token.Types.IDENT => "identifier",
            Token.Types.L_INT => "integer",
            Token.Types.L_FLOAT => "float",
            Token.Types.L_DOUBLE => "double",
            Token.Types.L_BOOL => "boolean",
            Token.Types.L_CHAR => "character",
            Token.Types.L_STRING => "string",
            Token.Types.L_NULL => "null",
            _ => type.ToString()
        };
    }

    /// <summary>
    /// Delegates the current index to to the handler
    /// Automatically moves the index on the end of the handler
    /// </summary>
    protected ParseNode? delegateToHandler(Handler other)
    {
        try
        {
            HandlerOutput output = other.HandleToken(tokens, i);
            i = output.endIndex;
            return output.node;
        }
        catch (SyntaxErrorException ex)
        {
            errorHandler.AddError(ex);
            while (HasMoreTokens && CurrentToken.Type != Token.Types.D_SEMICOLON && CurrentToken.Type != Token.Types.D_CBRAC_CLO)
            {
                incrementIndex();
            }
            if (HasMoreTokens && CurrentToken.Type == Token.Types.D_SEMICOLON)
            {
                incrementIndex();
            }
        }
        return null;
    }

    /// <summary>
    /// Tries to delegate to another handler
    /// </summary>
    /// <returns>True if delegation was successful, otherwise false</returns>
    protected ParseNode? tryDelegateToHandler(Handler other)
    {
        int originalIndex = i;
        errorHandler.SuppressErrors = true;
        HandlerOutput silentOutput;
        try
        {
            silentOutput = other.HandleToken(tokens, originalIndex);
        }
        catch (SyntaxErrorException)
        {
            i = originalIndex;
            return null;
        }
        finally
        {
            errorHandler.SuppressErrors = false;
        }

        // to allow for errors be saved 
        i = originalIndex;
        HandlerOutput output = other.HandleToken(tokens, i);
        i = output.endIndex;

        // rewind so callers can attempt alternatives
        if (output.node == null)
        {
            i = originalIndex;
            return null;
        }

        return output.node;
    }

    protected int getIndex() { return i; }

    protected void setIndex(int newIndex) { i = newIndex; }

    /// <summary>
    /// Increments the given index to the next
    /// </summary>
    protected void incrementIndex()
    {
        i++;
    }

    /// <summary>
    /// Skips to to the specified index 
    /// </summary>
    protected void skipToIndex(int i) { this.i = i; }
}
