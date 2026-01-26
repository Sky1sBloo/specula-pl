using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ExpressionStatementTests
{
    private ExpressionHandler expressionHandler;
    private ErrorsHandler errorsHandler;

    [SetUp]
    public void Setup()
    {
        errorsHandler = new();
        expressionHandler = new(errorsHandler);
    }

    [Test]
    public void BinaryExpressions()
    {
        var output = LexerFileReader.ParseFile("Samples/Expression/BinaryExpressions/Addition.json");
        HandlerOutput addNode = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(addNode.node, Is.TypeOf<AddExpression>()); 

        Setup(); // Reset for next assertion
        output = LexerFileReader.ParseFile("Samples/Expression/BinaryExpressions/Multiplication.json");
        HandlerOutput multNode = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(multNode.node, Is.TypeOf<MultExpression>());

        Setup(); // Reset for next assertion
        output = LexerFileReader.ParseFile("Samples/Expression/BinaryExpressions/Subtraction.json");
        HandlerOutput subNode = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(subNode.node, Is.TypeOf<SubExpression>());

        Setup(); // Reset for next assertion
        output = LexerFileReader.ParseFile("Samples/Expression/BinaryExpressions/Division.json");
        HandlerOutput divNode = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(divNode.node, Is.TypeOf<DivExpression>());
    }

    [Test]
    public void OperatorPrecedence()
    {
        // 2 + 3 * 4 = 2 + (3 * 4) = Add(2, Mult(3, 4))
        var output = LexerFileReader.ParseFile("Samples/Expression/PrecedenceExpressions/AddMultPrecedence.json");
        HandlerOutput node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<AddExpression>());
        var addExpr = (AddExpression)node.node!;
        Assert.That(addExpr.Rhs, Is.TypeOf<MultExpression>());

        // (2 + 3) * 4 = Mult(Add(2, 3), 4)
        output = LexerFileReader.ParseFile("Samples/Expression/PrecedenceExpressions/ParenthesisOverride.json");
        node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<MultExpression>());
        var multExpr = (MultExpression)node.node!;
        Assert.That(multExpr.Lhs, Is.TypeOf<AddExpression>());

        // 10 - 2 * 3 = 10 - (2 * 3) = Sub(10, Mult(2, 3))
        output = LexerFileReader.ParseFile("Samples/Expression/PrecedenceExpressions/SubMultPrecedence.json");
        node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<SubExpression>());
        var subExpr = (SubExpression)node.node!;
        Assert.That(subExpr.Rhs, Is.TypeOf<MultExpression>());
    }

    [Test]
    public void ParenthesesExpressions()
    {
        // (5 + 3) * 2
        var output = LexerFileReader.ParseFile("Samples/Expression/Parentheses/SimpleParenthesis.json");
        HandlerOutput node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<MultExpression>());

        // ((1 + 2) * 3) + 4
        output = LexerFileReader.ParseFile("Samples/Expression/Parentheses/NestedParenthesis.json");
        node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<AddExpression>());
        var addExpr = (AddExpression)node.node!;
        Assert.That(addExpr.Lhs, Is.TypeOf<MultExpression>());
    }

    [Test]
    public void FunctionCall()
    {
        var output = LexerFileReader.ParseFile("Samples/Expression/FunctionCall/FunctionCallNoParam.json");
        HandlerOutput node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void FunctionCallWithParam()
    {
        var output = LexerFileReader.ParseFile("Samples/Expression/FunctionCall/FunctionCallWithParam.json");
        HandlerOutput node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
    }
    
    [Test]
    public void UnaryPreIncrementDecrement()
    {
        // ++x
        var output = LexerFileReader.ParseFile("Samples/Expression/Unary/PreIncrement.json");
        HandlerOutput node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<PreIncExpression>());

        // --y
        output = LexerFileReader.ParseFile("Samples/Expression/Unary/PreDecrement.json");
        node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<PreDecExpression>());
    }

    [Test]
    public void UnaryPostIncrementDecrement()
    {
        // x++
        var output = LexerFileReader.ParseFile("Samples/Expression/Unary/PostIncrement.json");
        HandlerOutput node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<PostIncExpression>());

        // y--
        output = LexerFileReader.ParseFile("Samples/Expression/Unary/PostDecrement.json");
        node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<PostDecExpression>());
    }

    [Test]
    public void UnaryPositiveNegative()
    {
        // +5
        var output = LexerFileReader.ParseFile("Samples/Expression/Unary/UnaryPositive.json");
        HandlerOutput node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<PrePosExpression>());

        // -3
        output = LexerFileReader.ParseFile("Samples/Expression/Unary/UnaryNegative.json");
        node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<PreNegExpression>());
    }

    [Test]
    public void MixedUnaryBinary()
    {
        // ++x + 5
        var output = LexerFileReader.ParseFile("Samples/Expression/Mixed/PreIncrementAdd.json");
        HandlerOutput node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<AddExpression>());
        var addExpr = (AddExpression)node.node!;
        Assert.That(addExpr.Lhs, Is.TypeOf<PreIncExpression>());

        // x++ * 2
        Setup(); // Reset for next assertion
        output = LexerFileReader.ParseFile("Samples/Expression/Mixed/PostIncrementMult.json");
        node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<MultExpression>());
        var multExpr = (MultExpression)node.node!;
        Assert.That(multExpr.Lhs, Is.TypeOf<PostIncExpression>());
    }

    [Test]
    public void ComparisonExpressions()
    {
        // 5 == 3
        var output = LexerFileReader.ParseFile("Samples/Expression/Comparison/Equals.json");
        HandlerOutput node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<EqCompExpression>());

        // 5 < 3
        Setup(); // Reset for next assertion
        output = LexerFileReader.ParseFile("Samples/Expression/Comparison/LessThan.json");
        node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<LtCompExpression>());

        // 5 > 3
        Setup(); // Reset for next assertion
        output = LexerFileReader.ParseFile("Samples/Expression/Comparison/GreaterThan.json");
        node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<GtCompExpression>());

        // 5 <= 3
        Setup(); // Reset for next assertion
        output = LexerFileReader.ParseFile("Samples/Expression/Comparison/LessThanOrEqual.json");
        node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<LteCompExpression>());

        // 5 >= 3
        Setup(); // Reset for next assertion
        output = LexerFileReader.ParseFile("Samples/Expression/Comparison/GreaterThanOrEqual.json");
        node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<GteCompExpression>());
    }

    [Test]
    public void ComparisonPrecedence()
    {
        // 5 + 3 == 8 should be parsed as (5 + 3) == 8
        var output = LexerFileReader.ParseFile("Samples/Expression/Comparison/AdditionEqualsComparison.json");
        HandlerOutput node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<EqCompExpression>());
        var eqExpr = (EqCompExpression)node.node!;
        Assert.That(eqExpr.Lhs, Is.TypeOf<AddExpression>());

        // 10 - 2 < 5 should be parsed as (10 - 2) < 5
        Setup(); // Reset for next assertion
        output = LexerFileReader.ParseFile("Samples/Expression/Comparison/SubtractionLessComparison.json");
        node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<LtCompExpression>());
        var ltExpr = (LtCompExpression)node.node!;
        Assert.That(ltExpr.Lhs, Is.TypeOf<SubExpression>());

        // 3 * 4 > 10 should be parsed as (3 * 4) > 10
        Setup(); // Reset for next assertion
        output = LexerFileReader.ParseFile("Samples/Expression/Comparison/MultiplicationGreaterComparison.json");
        node = expressionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<GtCompExpression>());
        var gtExpr = (GtCompExpression)node.node!;
        Assert.That(gtExpr.Lhs, Is.TypeOf<MultExpression>());
    }
}
