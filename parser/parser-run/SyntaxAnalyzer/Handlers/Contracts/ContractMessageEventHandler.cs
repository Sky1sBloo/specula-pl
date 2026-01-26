using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class ContractMessageEventHandler : Handler
{
    public ContractMessageEventHandler(ErrorsHandler errorsHandler) : base(errorsHandler)
    {
    }

    protected override ParseNode? verifyTokens()
    {
        assertTokenType(Token.Types.IDENT);
        string messageEventName = CurrentToken.Value;
        incrementIndex();
        expectTokenType(Token.Types.D_CBRAC_OP);
        var parameters = parseParameters();
        expectTokenType(Token.Types.AT_SYMBOL);
        assertTokenType(Token.Types.IDENT);
        string initialState = CurrentToken.Value;
        incrementIndex();
        expectTokenType(Token.Types.OP_RIGHT_OP);
        assertTokenType(Token.Types.IDENT);
        string targetState = CurrentToken.Value;
        incrementIndex();
        expectTokenType(Token.Types.D_SEMICOLON);
        return new ContractMessageEventNode(
            messageEventName, 
            parameters, 
            new StateNode(initialState), 
            new StateNode(targetState)
        );
    }
    private FuncParams parseParameters()
    {
        var parameters = new PrintableList<FuncParam>();

        while (CurrentToken.Type != Token.Types.D_CBRAC_CLO)
        {
            assertTokenType(Token.Types.IDENT);
            string paramName = CurrentToken.Value;
            incrementIndex();

            expectTokenType(Token.Types.D_COLON);

            TypeDefinitionNode? paramType = parseParameterType();
            if (paramType != null)
            {
                parameters.Add(new FuncParam(paramName, paramType));
            }

            if (CurrentToken.Type == Token.Types.COMMA)
            {
                incrementIndex();
            }
            else if (CurrentToken.Type != Token.Types.D_CBRAC_CLO)
            {
                throw new SyntaxErrorException(["','", "'}'"], CurrentToken);
            }
        }
        incrementIndex();

        return new FuncParams(parameters);
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
