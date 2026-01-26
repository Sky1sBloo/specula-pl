using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class StructHandler: Handler
{
    private readonly VarDefinitionHandler varDefHandler;
    public StructHandler(ErrorsHandler errors) : base(errors)
    {
        varDefHandler = new VarDefinitionHandler(errors);
    }

    protected override ParseNode? verifyTokens()
    {
        expectTokenType(Token.Types.K_STRUCT);
        assertTokenType(Token.Types.IDENT);
        string structName = CurrentToken.Value;
        incrementIndex();
        expectTokenType(Token.Types.D_CBRAC_OP);
        PrintableList<StructField> fields = new();
        while (CurrentToken.Type != Token.Types.D_CBRAC_CLO)
        {
            expectTokenType(Token.Types.K_LET);
            assertTokenType(Token.Types.IDENT);
            string fieldName = CurrentToken.Value;
            incrementIndex();
            
            TypeDefinitionNode? fieldType = (TypeDefinitionNode?)delegateToHandler(varDefHandler);
            if (fieldType == null)
            {
                throw new SyntaxErrorException(["TYPE"], CurrentToken);
            }
            
            expectTokenType(Token.Types.D_SEMICOLON);
            fields.Add(new StructField(fieldName, fieldType));
        }

        incrementIndex();

        if (fields.Count == 0)
        {
            throw new SyntaxErrorException(
                ["At least one field in struct definition"],
                CurrentToken
            );
        }
        return new StructDefNode(structName, fields);
    }
}
