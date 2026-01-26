using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class DeclarationStatementTests
{
    private SyntaxAnalyzerRoot analyzer = new();

    [SetUp]
    public void Setup()
    {
        analyzer = new();
    }

    [Test]
    public void SingleDeclaration()
    {
        var output = LexerFileReader.ParseFile("Samples/Declaration/SingleDeclaration.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void SingleDeclarationWithValue()
    {
        var output = LexerFileReader.ParseFile("Samples/Declaration/SingleDeclarationWithValue.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void DeclarationInferType()
    {
        var output = LexerFileReader.ParseFile("Samples/Declaration/DeclarationInferType.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void MultipleDeclaration()
    {
        var output = LexerFileReader.ParseFile("Samples/Declaration/MultipleDeclaration.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(3));
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void ExpressionValues()
    {
        var output = LexerFileReader.ParseFile("Samples/Declaration/Expressions.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(7));
        }
    }
 

    [Test]
    public void NoIdentifier()
    {
        var output = LexerFileReader.ParseFile("Samples/Declaration/Invalid/NoIdentifier.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);

        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(0));
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(2));
    }

    [Test]
    public void NoType()
    {
        var output = LexerFileReader.ParseFile("Samples/Declaration/Invalid/NoType.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(0));
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(1));
    }

    [Test]
    public void InvalidOperatorsInDeclaration()
    {
        var output = LexerFileReader.ParseFile("Samples/Declaration/Invalid/InvalidOperators.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(0));
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(7));
    }
}
