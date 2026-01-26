using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class VarAssignTests
{
    private VarAssignHandler varAssignHandler;
    private ErrorsHandler errorsHandler;

    [SetUp]
    public void Setup()
    {
        errorsHandler = new();
        varAssignHandler = new(errorsHandler);
    }

    [Test]
    public void AssignmentOperators()
    {
        // x = 5
        var output = LexerFileReader.ParseFile("Samples/VarAssign/SimpleAssignment.json");
        HandlerOutput node = varAssignHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<AssignmentStatementNode>());
        var assign = (AssignmentStatementNode)node.node!;
        Assert.That(assign.Identifier, Is.EqualTo("x"));
        Assert.That(assign.Value, Is.TypeOf<LiteralValue>());

        // x += 3
        Setup();
        output = LexerFileReader.ParseFile("Samples/VarAssign/AddAssignment.json");
        node = varAssignHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<AssignPlusEqNode>());
        var addAssign = (AssignPlusEqNode)node.node!;
        Assert.That(addAssign.Identifier, Is.EqualTo("x"));

        // y -= 2
        Setup();
        output = LexerFileReader.ParseFile("Samples/VarAssign/SubtractAssignment.json");
        node = varAssignHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<AssignMinusEqNode>());
        var subAssign = (AssignMinusEqNode)node.node!;
        Assert.That(subAssign.Identifier, Is.EqualTo("y"));

        // z *= 4
        Setup();
        output = LexerFileReader.ParseFile("Samples/VarAssign/MultiplyAssignment.json");
        node = varAssignHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<AssignMulEqNode>());
        var mulAssign = (AssignMulEqNode)node.node!;
        Assert.That(mulAssign.Identifier, Is.EqualTo("z"));

        // w /= 2
        Setup();
        output = LexerFileReader.ParseFile("Samples/VarAssign/DivideAssignment.json");
        node = varAssignHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<AssignDivEqNode>());
        var divAssign = (AssignDivEqNode)node.node!;
        Assert.That(divAssign.Identifier, Is.EqualTo("w"));

        // m %= 3
        Setup();
        output = LexerFileReader.ParseFile("Samples/VarAssign/ModuloAssignment.json");
        node = varAssignHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<AssignModEqNode>());
        var modAssign = (AssignModEqNode)node.node!;
        Assert.That(modAssign.Identifier, Is.EqualTo("m"));
    }

    [Test]
    public void ExpressionAssignment()
    {
        // x = 5 + 3
        var output = LexerFileReader.ParseFile("Samples/VarAssign/ExpressionAssignment.json");
        HandlerOutput node = varAssignHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<AssignmentStatementNode>());
        var assignNode = (AssignmentStatementNode)node.node!;
        Assert.That(assignNode.Identifier, Is.EqualTo("x"));
        Assert.That(assignNode.Value, Is.TypeOf<AddExpression>());
        var addExpr = (AddExpression)assignNode.Value;
        Assert.That(addExpr.Lhs, Is.TypeOf<LiteralValue>());
        Assert.That(addExpr.Rhs, Is.TypeOf<LiteralValue>());
    }

    [Test]
    public void CompoundExpressionAssignment()
    {
        // x += 5 * 2 should parse as AddAssign with expression 5 * 2
        var output = LexerFileReader.ParseFile("Samples/VarAssign/CompoundExpressionAssignment.json");
        HandlerOutput node = varAssignHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<AssignPlusEqNode>());
        var assignNode = (AssignPlusEqNode)node.node!;
        Assert.That(assignNode.Identifier, Is.EqualTo("x"));
        Assert.That(assignNode.Value, Is.TypeOf<MultExpression>());
        var multExpr = (MultExpression)assignNode.Value;
        Assert.That(multExpr.Lhs, Is.TypeOf<LiteralValue>());
        Assert.That(multExpr.Rhs, Is.TypeOf<LiteralValue>());
    }

    [Test]
    public void MissingIdentifier()
    {
        // Missing identifier should throw error
        var tokens = new List<Token>
        {
            new() { Type = Token.Types.OP_EQUALS,Value = "=", Line = 1, CharStart = 1, CharEnd = 2 },
            new() { Type = Token.Types.L_INT,Value = "5", Line = 1, CharStart = 3, CharEnd = 4 }
        };

        Assert.Throws<SyntaxErrorException>(() => varAssignHandler.HandleToken(tokens, 0));
    }

    [Test]
    public void InvalidAssignmentNoRhs()
    {
        // Assignment without right-hand side should fail
        var tokens = new List<Token>
        {
            new() { Type = Token.Types.IDENT,Value = "x", Line = 1, CharStart = 1, CharEnd = 2 },
            new() { Type = Token.Types.OP_EQUALS,Value = "=", Line = 1, CharStart = 3, CharEnd = 4 }
            // Missing expression after equals
        };

        // Should throw an exception since there's no expression after the equals
        Assert.Throws<SyntaxErrorException>(() => varAssignHandler.HandleToken(tokens, 0));
    }
}
