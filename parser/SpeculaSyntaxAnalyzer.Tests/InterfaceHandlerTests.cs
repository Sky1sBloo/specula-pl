using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class InterfaceHandlerTests
{
    private SyntaxAnalyzerRoot analyzer = new();

    [SetUp]
    public void Setup()
    {
        analyzer = new();
    }

    [Test]
    public void SimpleInterface()
    {
        var output = LexerFileReader.ParseFile("Samples/Interface/Simple.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node, Is.Not.Null);
        var rootNode = (RootNode)node!;
        Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
        var interfaceNode = rootNode.Statements[0] as InterfaceDefNode;
        Assert.That(interfaceNode, Is.Not.Null);
        Assert.That(interfaceNode!.InterfaceName, Is.EqualTo("Simple"));
        Assert.That(interfaceNode.Methods.Count, Is.EqualTo(1));
        var method = interfaceNode.Methods[0] as InterfaceFuncReturnNode;
        Assert.That(method, Is.Not.Null);
        Assert.That(method!.Identifier, Is.EqualTo("method"));
    }

    [Test]
    public void InterfaceWithSelfReturnType()
    {
        var output = LexerFileReader.ParseFile("Samples/Interface/WIthSelf.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node, Is.Not.Null);
        var rootNode = (RootNode)node!;
        Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
        var interfaceNode = rootNode.Statements[0] as InterfaceDefNode;
        Assert.That(interfaceNode, Is.Not.Null);
        Assert.That(interfaceNode!.InterfaceName, Is.EqualTo("WithSelf"));
        Assert.That(interfaceNode.Methods.Count, Is.EqualTo(1));
        var method = interfaceNode.Methods[0] as InterfaceFuncReturnSelfNode;
        Assert.That(method, Is.Not.Null);
    }

    [Test]
    public void InterfaceWithSelfParameter()
    {
        var output = LexerFileReader.ParseFile("Samples/Interface/SelfParam.json");
        var result = analyzer.ReadTokens(output.Tokens) as RootNode;
        
        Assert.That(result?.Statements, Has.Count.EqualTo(1));
        var interfaceDef = result.Statements[0] as InterfaceDefNode;
        Assert.That(interfaceDef, Is.Not.Null);
        Assert.That(interfaceDef.InterfaceName, Is.EqualTo("SelfParam"));
        Assert.That(interfaceDef.Methods, Has.Count.EqualTo(1));
    }

    [Test]
    public void InterfaceWithMixedSelfAndRegularMethods()
    {
        var output = LexerFileReader.ParseFile("Samples/Interface/MixedSelf.json");
        var result = analyzer.ReadTokens(output.Tokens) as RootNode;
        
        Assert.That(result?.Statements, Has.Count.EqualTo(1));
        var interfaceDef = result.Statements[0] as InterfaceDefNode;
        Assert.That(interfaceDef, Is.Not.Null);
        Assert.That(interfaceDef.InterfaceName, Is.EqualTo("MultiMixed"));
        Assert.That(interfaceDef.Methods, Has.Count.EqualTo(2));
    }

    [Test]
    public void InterfaceWithMultipleMethods()
    {
        var output = LexerFileReader.ParseFile("Samples/Interface/MultiFunction.json");
        var result = analyzer.ReadTokens(output.Tokens) as RootNode;
        
        Assert.That(result?.Statements, Has.Count.EqualTo(1));
        var interfaceDef = result.Statements[0] as InterfaceDefNode;
        Assert.That(interfaceDef, Is.Not.Null);
        Assert.That(interfaceDef.InterfaceName, Is.EqualTo("Multi"));
        Assert.That(interfaceDef.Methods, Has.Count.EqualTo(2));
    }

    [Test]
    public void MissingMethodSemicolonThrows()
    {
        var output = LexerFileReader.ParseFile("Samples/Interface/Invalid/NoSemiColon.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.GreaterThan(0));
    }

    [Test]
    public void MergingFunctionsWithoutSemicolonThrows()
    {
        var output = LexerFileReader.ParseFile("Samples/Interface/Invalid/MergingFunctions.json");
        analyzer.ReadTokens(output.Tokens);
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.GreaterThan(0));
    }
}
