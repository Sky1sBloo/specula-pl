using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class FuncDefTests
{
    private SyntaxAnalyzerRoot analyzer = new();

    [SetUp]
    public void Setup()
    {
        analyzer = new();
    }

    [Test]
    public void BaseFunction()
    {
        var output = LexerFileReader.ParseFile("Samples/FuncDef/BaseFunction.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var funcDef = rootNode.Statements[0] as FuncDefNode;
            Assert.That(funcDef, Is.Not.Null);
            Assert.That(funcDef.Identifier, Is.EqualTo("greet"));
            Assert.That(funcDef.FunctionNode.Parameters.Params.Count, Is.EqualTo(0));
            Assert.That(funcDef.FunctionNode.ReturnType.DataType.DataType, Is.EqualTo(DataTypes.VOID));
            Assert.That(funcDef.IsAsync, Is.False);
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void FullDefinition()
    {
        var output = LexerFileReader.ParseFile("Samples/FuncDef/FullDefinition.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var funcDef = rootNode.Statements[0] as FuncDefNode;
            Assert.That(funcDef, Is.Not.Null);
            Assert.That(funcDef.Identifier, Is.EqualTo("compute"));
            Assert.That(funcDef.FunctionNode.Parameters.Params.Count, Is.EqualTo(1));
            Assert.That(funcDef.FunctionNode.Parameters.Params[0].Identifier, Is.EqualTo("x"));
            Assert.That(funcDef.FunctionNode.ReturnType.DataType.DataType, Is.EqualTo(DataTypes.STRING));
            Assert.That(funcDef.FunctionNode.ReturnType.Capabilities.CapabilityList.Any(c => c.Type == CapabilityTypes.THR_LOCAL), Is.True);
            Assert.That(funcDef.FunctionNode.ReturnType.Capabilities.CapabilityList.Any(c => c.Type == CapabilityTypes.MUT), Is.True);
            Assert.That(funcDef.IsAsync, Is.False);
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void ParamNoReturnType()
    {
        var output = LexerFileReader.ParseFile("Samples/FuncDef/ParamNoReturnType.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var funcDef = rootNode.Statements[0] as FuncDefNode;
            Assert.That(funcDef, Is.Not.Null);
            Assert.That(funcDef.Identifier, Is.EqualTo("process"));
            Assert.That(funcDef.FunctionNode.Parameters.Params.Count, Is.EqualTo(1));
            Assert.That(funcDef.FunctionNode.Parameters.Params[0].Identifier, Is.EqualTo("data"));
            Assert.That(funcDef.FunctionNode.ReturnType.DataType.DataType, Is.EqualTo(DataTypes.VOID));
            Assert.That(funcDef.IsAsync, Is.False);
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void ReturnTypeOnly()
    {
        var output = LexerFileReader.ParseFile("Samples/FuncDef/ReturnTypeOnly.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var funcDef = rootNode.Statements[0] as FuncDefNode;
            Assert.That(funcDef, Is.Not.Null);
            Assert.That(funcDef.Identifier, Is.EqualTo("getValue"));
            Assert.That(funcDef.FunctionNode.Parameters.Params.Count, Is.EqualTo(0));
            Assert.That(funcDef.FunctionNode.ReturnType.DataType.DataType, Is.EqualTo(DataTypes.STRING));
            Assert.That(funcDef.IsAsync, Is.False);
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void DefaultCapabilities()
    {
        var output = LexerFileReader.ParseFile("Samples/FuncDef/DefaultCapabilities.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var funcDef = rootNode.Statements[0] as FuncDefNode;
            Assert.That(funcDef, Is.Not.Null);
            Assert.That(funcDef.Identifier, Is.EqualTo("multiply"));
            Assert.That(funcDef.FunctionNode.Parameters.Params.Count, Is.EqualTo(2));
            Assert.That(funcDef.FunctionNode.Parameters.Params[0].Identifier, Is.EqualTo("a"));
            Assert.That(funcDef.FunctionNode.Parameters.Params[1].Identifier, Is.EqualTo("b"));
            Assert.That(funcDef.FunctionNode.ReturnType.DataType.DataType, Is.EqualTo(DataTypes.INT));
            Assert.That(funcDef.IsAsync, Is.False);
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void AsyncFunctionFullDefinition()
    {
        var output = LexerFileReader.ParseFile("Samples/FuncDef/AsyncFunctionFullDefinition.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var funcDef = rootNode.Statements[0] as FuncDefNode;
            Assert.That(funcDef, Is.Not.Null);
            Assert.That(funcDef.Identifier, Is.EqualTo("fetchData"));
            Assert.That(funcDef.FunctionNode.Parameters.Params.Count, Is.EqualTo(1));
            Assert.That(funcDef.FunctionNode.Parameters.Params[0].Identifier, Is.EqualTo("url"));
            Assert.That(funcDef.FunctionNode.ReturnType.DataType.DataType, Is.EqualTo(DataTypes.STRING));
            Assert.That(funcDef.FunctionNode.ReturnType.Capabilities.CapabilityList.Any(c => c.Type == CapabilityTypes.THR_LOCAL), Is.True);
            Assert.That(funcDef.IsAsync, Is.True);
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void AsyncMultiParam()
    {
        var output = LexerFileReader.ParseFile("Samples/FuncDef/AsyncMultiParam.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var funcDef = rootNode.Statements[0] as FuncDefNode;
            Assert.That(funcDef, Is.Not.Null);
            Assert.That(funcDef.Identifier, Is.EqualTo("transform"));
            Assert.That(funcDef.FunctionNode.Parameters.Params.Count, Is.EqualTo(2));
            Assert.That(funcDef.FunctionNode.Parameters.Params[0].Identifier, Is.EqualTo("input"));
            Assert.That(funcDef.FunctionNode.Parameters.Params[1].Identifier, Is.EqualTo("config"));
            Assert.That(funcDef.FunctionNode.ReturnType.DataType.DataType, Is.EqualTo(DataTypes.INT));
            Assert.That(funcDef.FunctionNode.ReturnType.Capabilities.CapabilityList.Any(c => c.Type == CapabilityTypes.THR_LOCAL), Is.True);
            Assert.That(funcDef.FunctionNode.ReturnType.Capabilities.CapabilityList.Any(c => c.Type == CapabilityTypes.MUT), Is.True);
            Assert.That(funcDef.IsAsync, Is.True);
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }

    [Test]
    public void AsyncNoParamsReturnType()
    {
        var output = LexerFileReader.ParseFile("Samples/FuncDef/AsyncNoParamsReturnType.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        if (node is RootNode rootNode)
        {
            Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
            var funcDef = rootNode.Statements[0] as FuncDefNode;
            Assert.That(funcDef, Is.Not.Null);
            Assert.That(funcDef.Identifier, Is.EqualTo("compute"));
            Assert.That(funcDef.FunctionNode.Parameters.Params.Count, Is.EqualTo(0));
            Assert.That(funcDef.FunctionNode.ReturnType.DataType.DataType, Is.EqualTo(DataTypes.VOID));
            Assert.That(funcDef.IsAsync, Is.True);
        }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
    }
}
