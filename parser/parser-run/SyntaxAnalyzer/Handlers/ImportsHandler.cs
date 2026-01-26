using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class ImportsHandler : Handler
{
    public ImportsHandler(ErrorsHandler errors) : base(errors)
    {
    }

    protected override ParseNode? verifyTokens()
    {
        var imports = new PrintableList<ImportModuleNode>();
        while (HasMoreTokens && CurrentToken.Type == Token.Types.K_IMPORT)
        {
            incrementIndex();
            switch (CurrentToken.Type)
            {
                case Token.Types.IDENT:
                    var importAlias = handleImportAlias();
                    if (importAlias != null)
                    {
                        imports.Add(importAlias);
                    }
                    break;
                case Token.Types.D_CBRAC_OP:
                    var importNode = handleImport();
                    if (importNode != null)
                    {
                        imports.Add(importNode);
                    }
                    break;
                default:
                    throw new SyntaxErrorException(["IDENTIFIER"], CurrentToken);
            }
        }
        return new ImportNodes(imports);
    }

    private ImportAliasNode? handleImportAlias()
    {
        string aliasName = CurrentToken.Value;
        incrementIndex();
        if (CurrentToken.Type != Token.Types.K_FROM)
        {
            throw new SyntaxErrorException(["from"], CurrentToken);
        }
        incrementIndex();
        if (CurrentToken.Type != Token.Types.L_STRING)
        {
            throw new SyntaxErrorException(["STRING"], CurrentToken);
        }
        string importPath = CurrentToken.Value;
        incrementIndex();
        if (CurrentToken.Type != Token.Types.D_SEMICOLON)
        {
            throw new SyntaxErrorException([";"], CurrentToken);
        }
        incrementIndex();
        return new ImportAliasNode(importPath, aliasName);
    }

    private ImportNode? handleImport()
    {
        if (CurrentToken.Type != Token.Types.D_CBRAC_OP)
        {
            throw new SyntaxErrorException(["{"], CurrentToken);
        }
        incrementIndex();
        var identifiers = new PrintableList<string>();
        while (CurrentToken.Type != Token.Types.D_CBRAC_CLO)
        {
            if (CurrentToken.Type != Token.Types.IDENT)
            {
                throw new SyntaxErrorException(["IDENTIFIER"], CurrentToken);
            }
            identifiers.Add(CurrentToken.Value);
            incrementIndex();
            if (CurrentToken.Type == Token.Types.COMMA)
            {
                incrementIndex();
            }
        }
        incrementIndex();
        if (CurrentToken.Type != Token.Types.K_FROM)
        {
            throw new SyntaxErrorException(["from"], CurrentToken);
        }
        incrementIndex();
        if (CurrentToken.Type != Token.Types.L_STRING)
        {
            throw new SyntaxErrorException(["STRING"], CurrentToken);
        }
        string importPath = CurrentToken.Value;
        incrementIndex();
        if (CurrentToken.Type != Token.Types.D_SEMICOLON)
        {
            throw new SyntaxErrorException([";"], CurrentToken);
        }
        incrementIndex();
        return new ImportNode(importPath, identifiers);
    }
}