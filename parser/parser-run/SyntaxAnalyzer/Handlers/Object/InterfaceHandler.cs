using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;



public class InterfaceHandler : Handler
{
    public InterfaceHandler(ErrorsHandler errors) : base(errors)
    {
    }

    protected override ParseNode? verifyTokens()
    {
        expectTokenType(Token.Types.K_INTERFACE);
        assertTokenType(Token.Types.IDENT);
        string interfaceName = CurrentToken.Value;
        incrementIndex();
        expectTokenType(Token.Types.D_CBRAC_OP);
        PrintableList<InterfaceFuncNode> methods = new();
        while (HasMoreTokens && CurrentToken.Type != Token.Types.D_CBRAC_CLO)
        {
            expectTokenType(Token.Types.K_FN);
            assertTokenType(Token.Types.IDENT);
            string funcName = CurrentToken.Value;
            incrementIndex();

            expectTokenType(Token.Types.D_PAR_OP);
            PrintableList<ParamNode> parameters = new();
            while (HasMoreTokens && CurrentToken.Type != Token.Types.D_PAR_CLO)
            {
                assertTokenType(Token.Types.IDENT);
                string paramName = CurrentToken.Value;
                incrementIndex();
                expectTokenType(Token.Types.D_COLON);
                
                if (CurrentToken.Type == Token.Types.K_SELF)
                {
                    parameters.Add(new InterfaceSelfParam(paramName));
                    incrementIndex();
                }
                else
                {
                    DataTypeHandler dataTypeHandler = new DataTypeHandler(errorHandler);
                    TypeNode? paramType = (TypeNode?)delegateToHandler(dataTypeHandler);
                    if (paramType == null)
                    {
                        throw new SyntaxErrorException(["TYPE"], CurrentToken);
                    }
                    Capabilities? capabilities = null;
                    if (CurrentToken.Type == Token.Types.D_BRAC_OP)
                    {
                        CapabilityHandler capabilityHandler = new CapabilityHandler(errorHandler);
                        capabilities = (Capabilities?)delegateToHandler(capabilityHandler);
                    }
                    if (capabilities == null)
                    {
                        capabilities = CapabilityHandler.GenerateDefaultCapabilities();
                    }
                    parameters.Add(new FuncParam(paramName, new TypeDefinitionNode(paramType, capabilities)));
                }

                if (CurrentToken.Type == Token.Types.COMMA)
                {
                    incrementIndex();
                }
            }
            expectTokenType(Token.Types.D_PAR_CLO);
            
            if (CurrentToken.Type != Token.Types.D_COLON)
            {
                // No return type specified, default to void
                expectTokenType(Token.Types.D_SEMICOLON);
                methods.Add(new InterfaceFuncReturnNode(funcName, parameters, new TypeDefinitionNode(new TypeNode(DataTypes.VOID), CapabilityHandler.GenerateDefaultCapabilities())));
            }
            else
            {
                // Consume colon and check for self or regular type
                incrementIndex();
                if (CurrentToken.Type == Token.Types.K_SELF)
                {
                    incrementIndex();
                    expectTokenType(Token.Types.D_SEMICOLON);
                    methods.Add(new InterfaceFuncReturnSelfNode(funcName, parameters));
                }
                else
                {
                    DataTypeHandler dataTypeHandler = new DataTypeHandler(errorHandler);
                    TypeNode? returnType = (TypeNode?)delegateToHandler(dataTypeHandler);
                    if (returnType == null)
                    {
                        return null;
                    }
                    Capabilities? capabilities = null;
                    if (CurrentToken.Type == Token.Types.D_BRAC_OP)
                    {
                        CapabilityHandler capabilityHandler = new CapabilityHandler(errorHandler);
                        capabilities = (Capabilities?)delegateToHandler(capabilityHandler);
                    }
                    if (capabilities == null)
                    {
                        capabilities = CapabilityHandler.GenerateDefaultCapabilities();
                    }

                    if (!HasMoreTokens)
                    {
                        throw new SyntaxErrorException([";"], new Token { Type = Token.Types.UNKNOWN, Line = 0, CharStart = 0, CharEnd = 0 });
                    }
                    expectTokenType(Token.Types.D_SEMICOLON);
                    methods.Add(new InterfaceFuncReturnNode(funcName, parameters, new TypeDefinitionNode(returnType, capabilities)));
                }
            }

        }
        if (!HasMoreTokens)
            throw new SyntaxErrorException(["}" ], new Token { Type = Token.Types.UNKNOWN });
        expectTokenType(Token.Types.D_CBRAC_CLO);
        return new InterfaceDefNode(interfaceName, methods);
    }
}