using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class StructHandlerTests
{
    private SyntaxAnalyzerRoot analyzer = new();

    [SetUp]
    public void Setup()
    {
        analyzer = new();
    }

    [Test]
    public void SimpleDefinitionParses()
    {
        var output = LexerFileReader.ParseFile("Samples/Struct/SimpleDefinition.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node, Is.Not.Null);
        Assert.That(node, Is.TypeOf<RootNode>());
        var rootNode = (RootNode)node!;
        Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
        var structNode = rootNode.Statements[0] as StructDefNode;
        Assert.That(structNode, Is.Not.Null);
        Assert.That(structNode!.StructName, Is.EqualTo("User"));
        Assert.That(structNode.Fields.Count, Is.EqualTo(2));

        Assert.That(structNode.Fields[0].Identifier, Is.EqualTo("id"));
        Assert.That(structNode.Fields[0].Definition.DataType.DataType, Is.EqualTo(DataTypes.INT));
        Assert.That(structNode.Fields[0].Definition.Capabilities.CapabilityList.Count, Is.EqualTo(2));
        Assert.That(structNode.Fields[0].Definition.Capabilities.CapabilityList[0].Type, Is.EqualTo(CapabilityTypes.OWN));
        Assert.That(structNode.Fields[0].Definition.Capabilities.CapabilityList[1].Type, Is.EqualTo(CapabilityTypes.CONST));

        Assert.That(structNode.Fields[1].Identifier, Is.EqualTo("name"));
        Assert.That(structNode.Fields[1].Definition.DataType.DataType, Is.EqualTo(DataTypes.STRING));
        Assert.That(structNode.Fields[1].Definition.Capabilities.CapabilityList.Count, Is.EqualTo(2));
        Assert.That(structNode.Fields[1].Definition.Capabilities.CapabilityList[0].Type, Is.EqualTo(CapabilityTypes.OWN));
        Assert.That(structNode.Fields[1].Definition.Capabilities.CapabilityList[1].Type, Is.EqualTo(CapabilityTypes.CONST));
    }

    [Test]
    public void StructWithCapabilities()
    {
        var output = LexerFileReader.ParseFile("Samples/Struct/CapabilityDefinition.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node, Is.Not.Null);
        var rootNode = (RootNode)node!;
        Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
        var structNode = rootNode.Statements[0] as StructDefNode;
        Assert.That(structNode, Is.Not.Null);
        Assert.That(structNode!.StructName, Is.EqualTo("Flags"));
        Assert.That(structNode.Fields.Count, Is.EqualTo(2));

        Assert.That(structNode.Fields[0].Identifier, Is.EqualTo("active"));
        Assert.That(structNode.Fields[0].Definition.DataType.DataType, Is.EqualTo(DataTypes.BOOL));
        Assert.That(structNode.Fields[0].Definition.Capabilities.CapabilityList.Count, Is.EqualTo(1));
        Assert.That(structNode.Fields[0].Definition.Capabilities.CapabilityList[0].Type, Is.EqualTo(CapabilityTypes.OWN));

        Assert.That(structNode.Fields[1].Identifier, Is.EqualTo("mode"));
        Assert.That(structNode.Fields[1].Definition.DataType.DataType, Is.EqualTo(DataTypes.STRING));
        Assert.That(structNode.Fields[1].Definition.Capabilities.CapabilityList.Count, Is.EqualTo(1));
        Assert.That(structNode.Fields[1].Definition.Capabilities.CapabilityList[0].Type, Is.EqualTo(CapabilityTypes.CONST));
    }

    [Test]
    public void NestedStructWithSharedCapability()
    {
        var output = LexerFileReader.ParseFile("Samples/Struct/Nested.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node, Is.Not.Null);
        var rootNode = (RootNode)node!;
        Assert.That(rootNode.Statements.Count, Is.EqualTo(1));
        var structNode = rootNode.Statements[0] as StructDefNode;
        Assert.That(structNode, Is.Not.Null);
        Assert.That(structNode!.StructName, Is.EqualTo("Nested"));
        Assert.That(structNode.Fields.Count, Is.EqualTo(1));

        Assert.That(structNode.Fields[0].Identifier, Is.EqualTo("inner"));
        Assert.That(structNode.Fields[0].Definition.DataType.DataType, Is.EqualTo(DataTypes.IDENTIFIER));
        Assert.That(structNode.Fields[0].Definition.Capabilities.CapabilityList.Count, Is.EqualTo(1));
        Assert.That(structNode.Fields[0].Definition.Capabilities.CapabilityList[0].Type, Is.EqualTo(CapabilityTypes.SHARED));
    }

    [Test]
    public void EmptyStructThrows()
    {
        var output = LexerFileReader.ParseFile("Samples/Struct/Invalid/EmptyStruct.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.GreaterThan(0), "Should have errors for empty struct");
    }

    [Test]
    public void MissingClosingBraceThrows()
    {
        var output = LexerFileReader.ParseFile("Samples/Struct/Invalid/MissingClosingBrace.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.GreaterThan(0), "Should have errors for missing closing brace");
    }

    [Test]
    public void TrailingCommaInCapabilitiesThrows()
    {
        var output = LexerFileReader.ParseFile("Samples/Struct/Invalid/TrailingComma.json");
        ParseNode? node = analyzer.ReadTokens(output.Tokens);
        Assert.That(analyzer.ErrorHandler.ErrorList.Count, Is.GreaterThan(0), "Should have errors for trailing comma in capabilities");
    }
}
