using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ImportsHandlerTests
{
    private SyntaxAnalyzerRoot analyzer = new();

    [SetUp]
    public void Setup()
    {
        analyzer = new();
    }

    [Test]
    public void ImportNamedExport()
    {
        var output = LexerFileReader.ParseFile("Samples/Import/ImportNamedExport.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var importNode = rootNode.Statements[0];
            Assert.That(importNode, Is.Not.Null);
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void ImportMultipleNamedExports()
    {
        var output = LexerFileReader.ParseFile("Samples/Import/ImportMultipleNamedExports.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var importNode = rootNode.Statements[0];
            Assert.That(importNode, Is.Not.Null);
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void ImportDefaultExport()
    {
        var output = LexerFileReader.ParseFile("Samples/Import/ImportDefaultExport.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var importNode = rootNode.Statements[0];
            Assert.That(importNode, Is.Not.Null);
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }
}
