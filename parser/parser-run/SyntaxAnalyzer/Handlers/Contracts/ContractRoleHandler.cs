using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class ContractRoleHandler : Handler
{
    public ContractRoleHandler(ErrorsHandler errorsHandler) : base(errorsHandler)
    {
    }

    protected override ParseNode? verifyTokens()
    {
        expectTokenType(Token.Types.K_ROLES);
        var rolesList = new PrintableList<RoleNode>();
        expectTokenType(Token.Types.D_COLON);
        
        assertTokenType(Token.Types.IDENT);
        rolesList.Add(new RoleNode(CurrentToken.Value));
        incrementIndex();
        
        while (HasMoreTokens && CurrentToken.Type == Token.Types.COMMA)
        {
            incrementIndex();
            assertTokenType(Token.Types.IDENT);
            rolesList.Add(new RoleNode(CurrentToken.Value));
            incrementIndex();
        }
        
        expectTokenType(Token.Types.D_SEMICOLON);
        return new RolesNode(rolesList);
    }
}