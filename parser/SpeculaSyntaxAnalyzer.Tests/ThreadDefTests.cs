using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ThreadDefTests
{
    private SyntaxAnalyzerRoot analyzer = new();

    [SetUp]
    public void Setup()
    {
        analyzer = new();
    }

    [Test]
    public void BaseThread()
    {
        var output = LexerFileReader.ParseFile("Samples/ThreadDef/BaseThread.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var threadDef = rootNode.Statements[0] as ThreadDefNode;
            Assert.That(threadDef, Is.Not.Null);
            Assert.That(threadDef.Identifier, Is.EqualTo("sampleThread"));
            Assert.That(threadDef.FunctionNode.Parameters.Params.Count, Is.EqualTo(0));
            Assert.That(threadDef.FunctionNode.ReturnType.DataType.DataType, Is.EqualTo(DataTypes.VOID));
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void FullDefinition()
    {
        var output = LexerFileReader.ParseFile("Samples/ThreadDef/FullDefinition.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var threadDef = rootNode.Statements[0] as ThreadDefNode;
            Assert.That(threadDef, Is.Not.Null);
            Assert.That(threadDef.Identifier, Is.EqualTo("sampleThread"));
            Assert.That(threadDef.FunctionNode.Parameters.Params.Count, Is.EqualTo(2));
            Assert.That(threadDef.FunctionNode.Parameters.Params[0].Identifier, Is.EqualTo("param1"));
            Assert.That(threadDef.FunctionNode.Parameters.Params[1].Identifier, Is.EqualTo("param2"));
            Assert.That(threadDef.FunctionNode.ReturnType.DataType.DataType, Is.EqualTo(DataTypes.VOID));
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void MultipleParameters()
    {
        var output = LexerFileReader.ParseFile("Samples/ThreadDef/BaseThread.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var threadDef = rootNode.Statements[0] as ThreadDefNode;
            Assert.That(threadDef, Is.Not.Null);
            Assert.That(threadDef.Identifier, Is.EqualTo("sampleThread"));
            Assert.That(threadDef.FunctionNode.Parameters.Params.Count, Is.EqualTo(0));
            Assert.That(threadDef.FunctionNode.ReturnType.DataType.DataType, Is.EqualTo(DataTypes.VOID));
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }
}
