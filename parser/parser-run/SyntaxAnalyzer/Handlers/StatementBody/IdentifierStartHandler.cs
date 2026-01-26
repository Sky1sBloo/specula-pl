using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

/// <summary>
/// Used for recognizing possible expressions starting with an identifier
/// </summary>
public class IdentifierStartHandler : Handler
{
    private readonly VarAssignHandler varAssignHandler;
    private readonly ExpressionHandler expressionHandler;
    public IdentifierStartHandler(ErrorsHandler errors) : base(errors)
    {
        varAssignHandler = new(errors);
        expressionHandler = new(errors);
    }

    protected override ParseNode? verifyTokens()
    {
        ParseNode? assignNode = tryDelegateToHandler(varAssignHandler);
        if (assignNode != null)
        {
            return assignNode;
        }

        ParseNode? exprNode = tryDelegateToHandler(expressionHandler);
        return exprNode;
    }

    protected ParseNode? handleAssignment(string identifier)
    {
        return delegateToHandler(varAssignHandler);
    }
}