using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class FuncShapeHandler: Handler
{
    private readonly VarDefinitionHandler varDefinitionHandler;
    private readonly BodyHandler bodyHandler;
    private readonly FuncParamsHandler paramsHandler;
    public FuncShapeHandler(ErrorsHandler errors) : base(errors)
    {
        varDefinitionHandler = new(errors);
        bodyHandler = new(errors);
        paramsHandler = new(errors, typesOptional: false);
    }

    protected override ParseNode? verifyTokens()
    {
        FuncParams? parameters = (FuncParams?)delegateToHandler(paramsHandler);
        if (parameters == null) return null;

        TypeDefinitionNode? returnType = getReturnType();
        if (returnType == null)
        {
            returnType = new TypeDefinitionNode(new TypeNode(DataTypes.VOID), CapabilityHandler.GenerateDefaultCapabilities());
        }
        BodyNode? body = parseFunctionBody();
        if (body == null)
            return null;
        return new FuncShapeNode(parameters, returnType, body);
    }

    private TypeDefinitionNode? getReturnType()
    {
        if (!HasMoreTokens || CurrentToken.Type != Token.Types.D_COLON)
        {
            return null;
        }
        return parseTypeDefinitionNode();
    }

    private TypeDefinitionNode? parseTypeDefinitionNode()
    {
        ParseNode? varDefNode = delegateToHandler(varDefinitionHandler);
        if (varDefNode == null)
        {
            return null;
        }
        return (TypeDefinitionNode)varDefNode;
    }

    private TypeDefinitionNode? parseParameterType()
    {
        // Parse type directly without expecting a leading colon
        // This is used for function parameters where the colon has already been consumed
        DataTypeHandler dataTypeHandler = new(errorHandler);
        TypeNode? dataType = (TypeNode?)delegateToHandler(dataTypeHandler);
        
        if (dataType == null)
        {
            return null;
        }

        // Check for capabilities
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

        // Default capabilities if none specified
        Capabilities defaultCapabilities = CapabilityHandler.GenerateDefaultCapabilities();
        return new TypeDefinitionNode(dataType, defaultCapabilities);
    }

    private BodyNode? parseFunctionBody()
    {
        ParseNode? bodyNode = delegateToHandler(bodyHandler);
        if (bodyNode == null)
        {
            return null;
        }
        return (BodyNode)bodyNode;
    }
}