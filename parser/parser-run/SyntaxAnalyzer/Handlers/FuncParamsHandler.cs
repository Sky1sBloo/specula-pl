using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

/// <summary>
/// Handles function parameters in parentheses: (param1, param2, param3)
/// Can optionally allow types to be omitted (using INFER datatype)
/// </summary>
public class FuncParamsHandler : Handler
{
    private readonly bool typesOptional;
    private readonly Token.Types openingToken;
    private readonly Token.Types closingToken;

    public FuncParamsHandler(ErrorsHandler err, 
        bool typesOptional = false,
        Token.Types openingToken = Token.Types.D_PAR_OP,
        Token.Types closingToken = Token.Types.D_PAR_CLO
        ) : base(err)
    {
        this.typesOptional = typesOptional;
        this.openingToken = openingToken;
        this.closingToken = closingToken;
    }

    protected override ParseNode? verifyTokens()
    {
        assertTokenType(openingToken);
        incrementIndex();

        PrintableList<FuncParam> parameters = parseParameters();

        assertTokenType(closingToken);
        incrementIndex();

        return new FuncParams(parameters);
    }

    private PrintableList<FuncParam> parseParameters()
    {
        var parameters = new PrintableList<FuncParam>();

        while (CurrentToken.Type != closingToken)
        {
            if (CurrentToken.Type == closingToken)
            {
                break;
            }
            
            assertTokenType(Token.Types.IDENT);
            string paramName = CurrentToken.Value;
            incrementIndex();

            TypeDefinitionNode? paramType;

            if (CurrentToken.Type == Token.Types.D_COLON)
            {
                incrementIndex();
                paramType = parseParameterType();
                if (paramType != null)
                {
                    parameters.Add(new FuncParam(paramName, paramType));
                }
                else
                {
                    // If type parsing failed, still add the parameter with inferred type if optional
                    if (typesOptional)
                    {
                        TypeDefinitionNode inferredType = new TypeDefinitionNode(
                            new TypeNode(DataTypes.INFER), 
                            CapabilityHandler.GenerateDefaultCapabilities()
                        );
                        parameters.Add(new FuncParam(paramName, inferredType));
                    }
                }
            }
            else if (typesOptional)
            {
                // Create an inferred type when types are optional and no colon present
                TypeDefinitionNode inferredType = new TypeDefinitionNode(
                    new TypeNode(DataTypes.INFER), 
                    CapabilityHandler.GenerateDefaultCapabilities()
                );
                parameters.Add(new FuncParam(paramName, inferredType));
            }
            else
            {
                throw new SyntaxErrorException([":"], CurrentToken);
            }

            if (CurrentToken.Type == Token.Types.COMMA)
            {
                incrementIndex();
                
                if (CurrentToken.Type == closingToken)
                {
                    throw new SyntaxErrorException(["parameter"], CurrentToken);
                }
            }
            else if (CurrentToken.Type != closingToken)
            {
                throw new SyntaxErrorException(["','", "')'"], CurrentToken);
            }
        }

        return parameters;
    }

    private TypeDefinitionNode? parseParameterType()
    {
        DataTypeHandler dataTypeHandler = new(errorHandler);
        TypeNode? dataType = (TypeNode?)delegateToHandler(dataTypeHandler);

        if (dataType == null)
        {
            return null;
        }

        if (HasMoreTokens && CurrentToken.Type == Token.Types.D_BRAC_OP)
        {
            CapabilityHandler capabilityHandler = new(errorHandler);
            Capabilities? capabilities = (Capabilities?)delegateToHandler(capabilityHandler);
            if (capabilities == null)
            {
                return null;
            }
            return new TypeDefinitionNode(dataType, capabilities);
        }

        Capabilities defaultCapabilities = CapabilityHandler.GenerateDefaultCapabilities();
        return new TypeDefinitionNode(dataType, defaultCapabilities);
    }
}
