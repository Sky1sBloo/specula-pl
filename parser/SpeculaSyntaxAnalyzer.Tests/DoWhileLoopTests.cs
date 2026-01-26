using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class DoWhileLoopTests
{
    private DoWhileLoopHandler doWhileHandler = null!;
    private ErrorsHandler errorsHandler = null!;

    [SetUp]
    public void Setup()
    {
        errorsHandler = new();
        doWhileHandler = new(errorsHandler);
    }

    #region Valid Cases

    [Test]
    public void DoWhileWithEmptyBody()
    {
        var output = LexerFileReader.ParseFile("Samples/While/DoWhile/DoWhileWIthEmptyBody.json");
        HandlerOutput result = doWhileHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(result.node, Is.InstanceOf<DoWhileLoop>());
        var doWhileLoop = (DoWhileLoop)result.node!;
        Assert.That(doWhileLoop.Body.Statements.Count, Is.EqualTo(0));
    }

    [Test]
    public void DoWhileWithStatements()
    {
        var output = LexerFileReader.ParseFile("Samples/While/DoWhile/DoWhileWithStatements.json");
        HandlerOutput result = doWhileHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(result.node, Is.InstanceOf<DoWhileLoop>());
        var doWhileLoop = (DoWhileLoop)result.node!;
        Assert.That(doWhileLoop.Body.Statements.Count, Is.GreaterThan(0));
    }

    [Test]
    public void NestedDoWhile()
    {
        var output = LexerFileReader.ParseFile("Samples/While/DoWhile/NestedDoWhile.json");
        HandlerOutput result = doWhileHandler.HandleToken(output.Tokens, 0);
        Assert.That(result.node, Is.InstanceOf<DoWhileLoop>());
        var doWhileLoop = (DoWhileLoop)result.node!;
        Assert.That(doWhileLoop.Body.Statements.Count, Is.GreaterThan(0));
        // Check that there's a nested do-while loop inside
        Assert.That(doWhileLoop.Body.Statements[0], Is.InstanceOf<DoWhileLoop>());
    }

    #endregion

    #region Invalid Cases

    [Test]
    public void MissingConditionParens_ShouldThrow()
    {
        var output = LexerFileReader.ParseFile("Samples/While/DoWhile/Invalid/MissingConditionParens.json");
        Assert.Throws<SyntaxErrorException>(() => doWhileHandler.HandleToken(output.Tokens, 0));
    }

    [Test]
    public void MissingCondition_ShouldThrow()
    {
        var output = LexerFileReader.ParseFile("Samples/While/DoWhile/Invalid/MissingCondition.json");
        Assert.Throws<SyntaxErrorException>(() => doWhileHandler.HandleToken(output.Tokens, 0));
    }

    [Test]
    public void MissingWhileKeyword_ShouldThrow()
    {
        var output = LexerFileReader.ParseFile("Samples/While/DoWhile/Invalid/MissingWhileKeyword.json");
        Assert.Throws<SyntaxErrorException>(() => doWhileHandler.HandleToken(output.Tokens, 0));
    }

    [Test]
    public void MissingConditionClosingParens_ShouldThrow()
    {
        var output = LexerFileReader.ParseFile("Samples/While/DoWhile/Invalid/MissingConditionClosingParens.json");
        Assert.Throws<SyntaxErrorException>(() => doWhileHandler.HandleToken(output.Tokens, 0));
    }

    #endregion
}
