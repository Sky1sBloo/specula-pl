using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class CapabilityHandler : Handler
{
    public CapabilityHandler(ErrorsHandler errors) : base(errors) { }

    public static Capabilities GenerateDefaultCapabilities()
    {
        return new Capabilities(new PrintableList<Capability>()
        {
            new Capability(CapabilityTypes.OWN, new PrintableList<string>()),
            new Capability(CapabilityTypes.CONST, new PrintableList<string>())
        });
    }

    protected override ParseNode? verifyTokens()
    {
        if (CurrentToken.Type != Token.Types.D_BRAC_OP)
        {
            throw new SyntaxErrorException(["["], CurrentToken);
        }
        incrementIndex();
        PrintableList<Capability> capabilities = new();

        // Check for empty capability list []
        if (CurrentToken.Type == Token.Types.D_BRAC_CLO)
        {
            var ex = new SyntaxErrorException(["capability"], CurrentToken);
            errorHandler.AddError(ex);
            throw ex;
        }

        while (true)
        {
            CapabilityTypes type;
            PrintableList<string> configuration = new();

            if (!HasMoreTokens)
            {
                Token missing = new()
                {
                    Type = Token.Types.UNKNOWN,
                    Line = getIndex(),
                    CharStart = 0,
                    CharEnd = 0
                };
                throw new SyntaxErrorException(["]"], missing);
            }

            if (CurrentToken.Type == Token.Types.D_BRAC_CLO)
            {
                break;
            }

            switch (CurrentToken.Type)
            {
                case Token.Types.K_OWN:
                    type = CapabilityTypes.OWN;
                    break;
                case Token.Types.K_MOVE:
                    type = CapabilityTypes.MOVE;
                    break;
                case Token.Types.K_SHARED:
                    type = CapabilityTypes.SHARED;
                    break;
                case Token.Types.K_VIEW:
                    type = CapabilityTypes.VIEW;
                    break;
                case Token.Types.K_SHARE:
                    type = CapabilityTypes.SHARE;
                    break;
                case Token.Types.K_MUT:
                    type = CapabilityTypes.MUT;
                    break;
                case Token.Types.K_CONST:
                    type = CapabilityTypes.CONST;
                    break;
                case Token.Types.K_THR_LOCAL:
                    type = CapabilityTypes.THR_LOCAL;
                    break;
                case Token.Types.K_SYNC:
                    type = CapabilityTypes.SYNC;
                    break;
                case Token.Types.K_INFER:
                    type = CapabilityTypes.INFER;
                    break;
                case Token.Types.K_NETWORK:
                    type = CapabilityTypes.NETWORK;
                    break;
                default:
                    var ex = new SyntaxErrorException(["OWN", "CONST", "REF"], CurrentToken);
                    errorHandler.AddError(ex);
                    throw ex;
            }
            incrementIndex();
            // For [own, network[json]]
            if (CurrentToken.Type == Token.Types.D_BRAC_OP)
            {
                incrementIndex();
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
                        throw new SyntaxErrorException(["]"], missing);
                    }

                    if (CurrentToken.Type == Token.Types.D_BRAC_CLO)
                        break;

                    if (CurrentToken.Type == Token.Types.IDENT || CurrentToken.Type == Token.Types.K_THIS)
                    {
                        configuration.Add(CurrentToken.Value);
                        incrementIndex();
                    }
                    else if (IsCapabilityKeyword(CurrentToken.Type))
                    {
                        // Reject stacked capabilities like shared[shared]
                        var ex = new SyntaxErrorException(["IDENTIFIER"], CurrentToken);
                        errorHandler.AddError(ex);
                        throw ex;
                    }
                    else
                        throw new SyntaxErrorException(["IDENTIFIER"], CurrentToken);
                }
                incrementIndex();
            }
            capabilities.Add(new Capability(type, configuration));

            //  For comma between capabilities
            if (HasMoreTokens && CurrentToken.Type == Token.Types.COMMA)
            {
                incrementIndex();

                // Check for trailing comma
                if (!HasMoreTokens || CurrentToken.Type == Token.Types.D_BRAC_CLO)
                {
                    Token missing = new()
                    {
                        Type = Token.Types.UNKNOWN,
                        Line = getIndex(),
                        CharStart = 0,
                        CharEnd = 0
                    };
                    var ex = new SyntaxErrorException(["capability"], missing);
                    errorHandler.AddError(ex);
                    throw ex;
                }
                continue;
            }
            else if (HasMoreTokens && CurrentToken.Type != Token.Types.D_BRAC_CLO)
            {
                throw new SyntaxErrorException([",", "]"], CurrentToken);
            }
        }
        incrementIndex();
        return new Capabilities(capabilities);
    }

    private bool IsCapabilityKeyword(Token.Types type)
    {
        return type == Token.Types.K_OWN ||
               type == Token.Types.K_MOVE ||
               type == Token.Types.K_SHARED ||
               type == Token.Types.K_VIEW ||
               type == Token.Types.K_SHARE ||
               type == Token.Types.K_MUT ||
               type == Token.Types.K_CONST ||
               type == Token.Types.K_THR_LOCAL ||
               type == Token.Types.K_SYNC ||
               type == Token.Types.K_INFER ||
               type == Token.Types.K_NETWORK;
    }
}