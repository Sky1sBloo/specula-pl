using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ExportHandlerTests
{
    private SyntaxAnalyzerRoot analyzer = new();

    [SetUp]
    public void Setup()
    {
        analyzer = new();
    }

    [Test]
    public void ExportFunction()
    {
        var output = LexerFileReader.ParseFile("Samples/Export/ExportFunction.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var exportNode = rootNode.Statements[0] as ExportNode;
            Assert.That(exportNode, Is.Not.Null);
            if (exportNode is not null)
            {
                var funcDef = exportNode.Statement as FuncDefNode;
                Assert.That(funcDef, Is.Not.Null);
                Assert.That(funcDef?.Identifier, Is.EqualTo("toExport"));
            }
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void ExportDefaultFunction()
    {
        var output = LexerFileReader.ParseFile("Samples/Export/ExportDefaultFunction.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var exportNode = rootNode.Statements[0] as ExportDefaultNode;
            Assert.That(exportNode, Is.Not.Null);
            if (exportNode is not null)
            {
                var funcDef = exportNode.Statement as FuncDefNode;
                Assert.That(funcDef, Is.Not.Null);
                Assert.That(funcDef?.Identifier, Is.EqualTo("defaultFunc"));
            }
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void ExportVariable()
    {
        var output = LexerFileReader.ParseFile("Samples/Export/ExportVariable.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var exportNode = rootNode.Statements[0] as ExportNode;
            Assert.That(exportNode, Is.Not.Null);
            if (exportNode is not null)
            {
                var varDef = exportNode.Statement as DeclarationStatementNode;
                Assert.That(varDef, Is.Not.Null);
                Assert.That(varDef?.Identifier, Is.EqualTo("myVar"));
            }
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void ExportDefaultVariable()
    {
        var output = LexerFileReader.ParseFile("Samples/Export/ExportDefaultVariable.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var exportNode = rootNode.Statements[0] as ExportDefaultNode;
            Assert.That(exportNode, Is.Not.Null);
            if (exportNode is not null)
            {
                var varDef = exportNode.Statement as DeclarationStatementNode;
                Assert.That(varDef, Is.Not.Null);
                Assert.That(varDef?.Identifier, Is.EqualTo("defaultVar"));
            }
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }
}
