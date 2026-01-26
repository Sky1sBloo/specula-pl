using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ImplTests
{
    private SyntaxAnalyzerRoot analyzer = new();

    [SetUp]
    public void Setup()
    {
        analyzer = new();
    }

    // Valid samples
    [Test]
    public void Impl_EmptyBody_Parses()
    {
        var output = LexerFileReader.ParseFile("Samples/Impl/EmptyBody.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
        var root = node as RootNode;
        Assert.That(root, Is.Not.Null);
        Assert.That(root!.Statements.Count, Is.EqualTo(1));
        var implDef = root!.Statements[0] as ImplDefNode;
        Assert.That(implDef, Is.Not.Null);
        Assert.That(implDef!.InterfaceName, Is.EqualTo("I"));
        Assert.That(implDef!.StructName, Is.EqualTo("S"));
        Assert.That(implDef!.Methods.Count, Is.EqualTo(1));
        var func = implDef!.Methods[0] as FuncDefNode;
        Assert.That(func, Is.Not.Null);
        Assert.That(func!.Identifier, Is.EqualTo("ping"));
        Assert.That(func!.FunctionNode.Parameters.Params.Count, Is.EqualTo(0));
        Assert.That(func!.FunctionNode.ReturnType.DataType.DataType, Is.EqualTo(DataTypes.VOID));
        Assert.That(func!.FunctionNode.Body.Statements.Count, Is.EqualTo(0));
    }

    [Test]
    public void Impl_WithReturnType_Parses()
    {
        var output = LexerFileReader.ParseFile("Samples/Impl/WithReturnType.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
        var root = node as RootNode;
        Assert.That(root, Is.Not.Null);
        var implDef = root!.Statements[0] as ImplDefNode;
        Assert.That(implDef, Is.Not.Null);
        Assert.That(implDef!.InterfaceName, Is.EqualTo("Math"));
        Assert.That(implDef!.StructName, Is.EqualTo("S"));
        Assert.That(implDef!.Methods.Count, Is.EqualTo(1));
        var func = implDef!.Methods[0] as FuncDefNode;
        Assert.That(func, Is.Not.Null);
        Assert.That(func!.Identifier, Is.EqualTo("add"));
        Assert.That(func!.FunctionNode.Parameters.Params.Count, Is.EqualTo(2));
        Assert.That(func!.FunctionNode.Parameters.Params[0].Identifier, Is.EqualTo("x"));
        Assert.That(func!.FunctionNode.Parameters.Params[1].Identifier, Is.EqualTo("y"));
        Assert.That(func!.FunctionNode.ReturnType.DataType.DataType, Is.EqualTo(DataTypes.INT));
    }

    [Test]
    public void Impl_MultiMethod_Parses()
    {
        var output = LexerFileReader.ParseFile("Samples/Impl/MultiMethod.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(node, Is.Not.Null);
        var root = node as RootNode;
        Assert.That(root, Is.Not.Null);
        var implDef = root!.Statements[0] as ImplDefNode;
        Assert.That(implDef, Is.Not.Null);
        Assert.That(implDef!.InterfaceName, Is.EqualTo("Math"));
        Assert.That(implDef!.StructName, Is.EqualTo("S"));
        Assert.That(implDef!.Methods.Count, Is.EqualTo(3));
    }

    // Invalid samples
    [Test]
    public void Impl_Invalid_SelfInParam_Throws()
    {
        var output = LexerFileReader.ParseFile("Samples/Impl/Invalid/SelfInParam.json");
        ParseNode? node = null;
        try { node = analyzer.ReadTokens(output.Tokens); } catch { /* errors are collected in ErrorHandler */ }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void Impl_Invalid_NoBody_Throws()
    {
        var output = LexerFileReader.ParseFile("Samples/Impl/Invalid/NoBody.json");
        ParseNode? node = null;
        try { node = analyzer.ReadTokens(output.Tokens); } catch { /* errors are collected in ErrorHandler */ }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void Impl_Invalid_MissingClosingBrace_Throws()
    {
        var output = LexerFileReader.ParseFile("Samples/Impl/Invalid/MissingClosingBrace.json");
        ParseNode? node = null;
        try { node = analyzer.ReadTokens(output.Tokens); } catch { /* errors are collected in ErrorHandler */ }
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.GreaterThanOrEqualTo(1));
    }
}
