using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class DeclarationHandler : Handler
{
    private readonly VarDefinitionHandler varDefinitionHandler;
    private readonly ExpressionHandler expressionHandler;

    private string identifier = "";
    private TypeDefinitionNode? typeDefinition = null;
    private Expression? value = null;

    public DeclarationHandler(ErrorsHandler errors) : base(errors)
    {
        varDefinitionHandler = new(errors);
        expressionHandler = new(errors);
    }

    public void Reset()
    {
        identifier = "";
        typeDefinition = null;
        value = null;
    }

    protected override ParseNode? verifyTokens()
    {
        expectTokenType(Token.Types.K_LET);
        assertTokenType(Token.Types.IDENT);
        identifier = CurrentToken.Value;
        incrementIndex();
        switch (CurrentToken.Type)
        {
            case Token.Types.D_COLON:
                return sawColonState();
            case Token.Types.OP_EQUALS:
                incrementIndex();
                return sawEqualsState();
        }
        throw new SyntaxErrorException([":", "="], CurrentToken);
    }

    private ParseNode? sawColonState()
    {
        typeDefinition = getTypeDefinition();

        switch (CurrentToken.Type)
        {
            case Token.Types.OP_EQUALS:
                incrementIndex();
                return sawEqualsState();
            case Token.Types.D_SEMICOLON:
                {
                    if (typeDefinition == null) throw new SyntaxErrorException(["Definition of datatype"], CurrentToken);
                    value = new LiteralValue(new TypeNode(DataTypes.NULL), "null");
                    return ConstructNode();
                }
            default:
                throw new SyntaxErrorException([";", "="], CurrentToken);
        }
    }

    private ParseNode? sawEqualsState()
    {
        value = getVariableValue();
        if (value == null) return null;

        if (typeDefinition == null)
        {
            TypeNode inferredType = InferTypeFromExpression(value);
            Capabilities defaultCapabilities = generateDefaultCapabilities();
            typeDefinition = new TypeDefinitionNode(inferredType, defaultCapabilities);
        }

        return ConstructNode();
    }

    private TypeNode InferTypeFromExpression(Expression expr)
    {
        if (expr is LiteralValue literal)
        {
            return literal.Type;
        }

        if (expr is IdentifierValue)
        {
            return new TypeNode(DataTypes.IDENTIFIER);
        }

        return new TypeNode(DataTypes.UNKNOWN);
    }

    private DeclarationStatementNode ConstructNode()
    {
        if (typeDefinition == null || value == null)
        {
            throw new InvalidOperationException("Tried to construct node of null datatype or value");
        }
        expectTokenType(Token.Types.D_SEMICOLON);
        DeclarationStatementNode statementNode = new(identifier, typeDefinition, value);
        Reset();
        return statementNode;
    }

    private TypeDefinitionNode? getTypeDefinition()
    {
        ParseNode? parseNode = delegateToHandler(varDefinitionHandler);
        if (parseNode == null) return null;
        return (TypeDefinitionNode)parseNode;
    }

    private Expression? getVariableValue()
    {
        ParseNode? parseNode = delegateToHandler(expressionHandler);

        if (parseNode == null) return null;
        return (Expression)parseNode;
    }

    private Capabilities generateDefaultCapabilities()
    {
        return new Capabilities(new PrintableList<Capability>()
        {
            new Capability(CapabilityTypes.OWN, new PrintableList<string>()),
            new Capability(CapabilityTypes.CONST, new PrintableList<string>())
        });
    }
}
