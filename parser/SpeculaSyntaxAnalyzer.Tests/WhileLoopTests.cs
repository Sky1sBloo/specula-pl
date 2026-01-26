using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class WhileLoopTests
{
    private WhileLoopHandler whileHandler = null!;
    private ErrorsHandler errorsHandler = null!;

    [SetUp]
    public void Setup()
    {
        errorsHandler = new();
        whileHandler = new(errorsHandler);
    }

    #region Valid Cases

    [Test]
    public void WhileWithEmptyBody()
    {
        var output = LexerFileReader.ParseFile("Samples/While/WhileWithEmptyBody.json");
        HandlerOutput result = whileHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(result.node, Is.InstanceOf<WhileLoop>());
        var whileLoop = (WhileLoop)result.node!;
        Assert.That(whileLoop.Body.Statements.Count, Is.EqualTo(0));
    }

    [Test]
    public void WhileWithStatements()
    {
        var output = LexerFileReader.ParseFile("Samples/While/WhileWIthStatements.json");
        HandlerOutput result = whileHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(result.node, Is.InstanceOf<WhileLoop>());
        var whileLoop = (WhileLoop)result.node!;
        Assert.That(whileLoop.Body.Statements.Count, Is.GreaterThan(0));
    }

    [Test]
    public void NestedWhile()
    {
        var output = LexerFileReader.ParseFile("Samples/While/NestedWhile.json");
        HandlerOutput result = whileHandler.HandleToken(output.Tokens, 0);
        Assert.That(result.node, Is.InstanceOf<WhileLoop>());
        var whileLoop = (WhileLoop)result.node!;
        Assert.That(whileLoop.Body.Statements.Count, Is.GreaterThan(0));
        // Check that there's a nested while loop inside
        Assert.That(whileLoop.Body.Statements[0], Is.InstanceOf<WhileLoop>());
    }

    #endregion

    #region Invalid Cases

    [Test]
    public void MissingConditionParens_ShouldThrow()
    {
        var output = LexerFileReader.ParseFile("Samples/While/Invalid/MissingConditionParens.json");
        Assert.Throws<SyntaxErrorException>(() => whileHandler.HandleToken(output.Tokens, 0));
    }

    [Test]
    public void MissingCondition_ShouldThrow()
    {
        var output = LexerFileReader.ParseFile("Samples/While/Invalid/MissingCondition.json");
        Assert.Throws<SyntaxErrorException>(() => whileHandler.HandleToken(output.Tokens, 0));
    }

    [Test]
    public void MissingClosingParen_ShouldThrow()
    {
        var output = LexerFileReader.ParseFile("Samples/While/Invalid/MissingClosingParen.json");
        Assert.Throws<SyntaxErrorException>(() => whileHandler.HandleToken(output.Tokens, 0));
    }

    #endregion
}
