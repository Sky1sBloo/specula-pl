using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ForLoopTests
{
    private ForLoopHandler forHandler = null!;
    private ErrorsHandler errorsHandler = null!;

    [SetUp]
    public void Setup()
    {
        errorsHandler = new();
        forHandler = new(errorsHandler);
    }

    #region Valid Cases

    [Test]
    public void ForInitAssignment()
    {
        var output = LexerFileReader.ParseFile("Samples/For/ForInitAssignment.json");
        HandlerOutput result = forHandler.HandleToken(output.Tokens, 0);
        Assert.That(result.node, Is.InstanceOf<ForLoop>());
        var forLoop = (ForLoop)result.node!;
        Assert.That(forLoop.Init, Is.InstanceOf<AssignmentStatementNode>());
        Assert.That(forLoop.Assignment, Is.InstanceOf<AssignmentStatementNode>());
    }

    [Test]
    public void ForInitDeclaration()
    {
        var output = LexerFileReader.ParseFile("Samples/For/ForInitDeclaration.json");
        HandlerOutput result = forHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(result.node, Is.InstanceOf<ForLoop>());
        var forLoop = (ForLoop)result.node!;
        Assert.That(forLoop.Init, Is.InstanceOf<DeclarationStatementNode>());
        Assert.That(forLoop.Assignment, Is.InstanceOf<AssignmentStatementNode>());
    }

    [Test]
    public void ForWithIncrement()
    {
        var output = LexerFileReader.ParseFile("Samples/For/ForIncrement.json");
        HandlerOutput result = forHandler.HandleToken(output.Tokens, 0);
        Assert.That(result.node, Is.InstanceOf<ForLoop>());
        var forLoop = (ForLoop)result.node!;
        Assert.That(forLoop.Init, Is.InstanceOf<AssignmentStatementNode>());
        Assert.That(forLoop.Assignment, Is.InstanceOf<AssignmentStatementNode>());
    }

    [Test]
    public void ForWithCompoundAssignment()
    {
        var output = LexerFileReader.ParseFile("Samples/For/ForWithCompoundAssignment.json");
        HandlerOutput result = forHandler.HandleToken(output.Tokens, 0);
        Assert.That(result.node, Is.InstanceOf<ForLoop>());
        var forLoop = (ForLoop)result.node!;
        Assert.That(forLoop.Init, Is.InstanceOf<DeclarationStatementNode>());
        Assert.That(forLoop.Assignment, Is.InstanceOf<AssignPlusEqNode>());
    }

    [Test]
    public void ForWithComplexCondition()
    {
        var output = LexerFileReader.ParseFile("Samples/For/ForComplexCondition.json");
        HandlerOutput result = forHandler.HandleToken(output.Tokens, 0);
        
        foreach (var error in errorsHandler.ErrorList)
        {
            Console.WriteLine($"Error: {error}");
        }
        
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(result.node, Is.InstanceOf<ForLoop>());
        var forLoop = (ForLoop)result.node!;
        Assert.That(forLoop.Init, Is.InstanceOf<DeclarationStatementNode>());
        Assert.That(forLoop.Assignment, Is.InstanceOf<AssignPlusEqNode>());
    }

    [Test]
    public void ForWithBody()
    {
        var output = LexerFileReader.ParseFile("Samples/For/ForWithBody.json");
        HandlerOutput result = forHandler.HandleToken(output.Tokens, 0);
        Assert.That(result.node, Is.InstanceOf<ForLoop>());
    }

    #endregion

    #region Invalid Cases

    [Test]
    public void MissingForKeyword_ShouldThrow()
    {
        var output = LexerFileReader.ParseFile("Samples/For/Invalid/MissingForKeyword.json");
        Assert.Throws<SyntaxErrorException>(() => forHandler.HandleToken(output.Tokens, 0));
    }

    [Test]
    public void MissingOpenParenthesis_ShouldThrow()
    {
        var output = LexerFileReader.ParseFile("Samples/For/Invalid/MissingOpenParenthesis.json");
        Assert.Throws<SyntaxErrorException>(() => forHandler.HandleToken(output.Tokens, 0));
    }

    [Test]
    public void MissingCloseParenthesis_ShouldThrow()
    {
        var output = LexerFileReader.ParseFile("Samples/For/Invalid/MissingCloseParenthesis.json");
        Assert.Throws<SyntaxErrorException>(() => forHandler.HandleToken(output.Tokens, 0));
    }

    [Test]
    public void MissingFirstSemicolon_ShouldThrow()
    {
        var output = LexerFileReader.ParseFile("Samples/For/Invalid/MissingFirstSemicolon.json");
        Assert.Throws<SyntaxErrorException>(() => forHandler.HandleToken(output.Tokens, 0));
    }

    [Test]
    public void MissingSecondSemicolon_ShouldThrow()
    {
        var output = LexerFileReader.ParseFile("Samples/For/Invalid/MissingSecondSemicolon.json");
        Assert.Throws<SyntaxErrorException>(() => forHandler.HandleToken(output.Tokens, 0));
    }

    #endregion
}
