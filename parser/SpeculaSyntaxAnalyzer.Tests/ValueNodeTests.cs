using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ValueNodeTests
{
    private ValueHandler valueHandler;
    private ErrorsHandler errorsHandler;

    [SetUp]
    public void Setup()
    {
        errorsHandler = new();
        valueHandler = new(errorsHandler);
    }

    [Test]
    public void LiteralIntValue()
    {
        var output = LexerFileReader.ParseFile("Samples/Value/LiteralInt.json");
        HandlerOutput node = valueHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<LiteralValue>());
        
        var literalValue = (LiteralValue)node.node!;
        Assert.That(literalValue.Type.DataType, Is.EqualTo(DataTypes.INT));
        Assert.That(literalValue.Value, Is.EqualTo("42"));
    }

    [Test]
    public void LiteralStringValue()
    {
        var output = LexerFileReader.ParseFile("Samples/Value/LiteralString.json");
        HandlerOutput node = valueHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<LiteralValue>());
        
        var literalValue = (LiteralValue)node.node!;
        Assert.That(literalValue.Type.DataType, Is.EqualTo(DataTypes.STRING));
        Assert.That(literalValue.Value, Is.EqualTo("\"hello\""));
    }

    [Test]
    public void LiteralBoolValue()
    {
        var output = LexerFileReader.ParseFile("Samples/Value/LiteralBool.json");
        HandlerOutput node = valueHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<LiteralValue>());
        
        var literalValue = (LiteralValue)node.node!;
        Assert.That(literalValue.Type.DataType, Is.EqualTo(DataTypes.BOOL));
        Assert.That(literalValue.Value, Is.EqualTo("true"));
    }

    [Test]
    public void LiteralFloatValue()
    {
        var output = LexerFileReader.ParseFile("Samples/Value/LiteralFloat.json");
        HandlerOutput node = valueHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<LiteralValue>());
        
        var literalValue = (LiteralValue)node.node!;
        Assert.That(literalValue.Type.DataType, Is.EqualTo(DataTypes.FLOAT));
        Assert.That(literalValue.Value, Is.EqualTo("3.14f"));
    }

    [Test]
    public void IdentifierValue()
    {
        var output = LexerFileReader.ParseFile("Samples/Value/Identifier.json");
        HandlerOutput node = valueHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<IdentifierValue>());
        
        var identifierValue = (IdentifierValue)node.node!;
        Assert.That(identifierValue.Value, Is.EqualTo("myVar"));
    }

    [Test]
    public void FunctionCallNoParameters()
    {
        var output = LexerFileReader.ParseFile("Samples/Value/FunctionCallNoParams.json");
        HandlerOutput node = valueHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<FunctionCallValue>());
        
        var funcCall = (FunctionCallValue)node.node!;
        Assert.That(funcCall.Identifier, Is.EqualTo("print"));
        Assert.That(funcCall.Parameters.Count, Is.EqualTo(0));
    }

    [Test]
    public void FunctionCallOneParameter()
    {
        var output = LexerFileReader.ParseFile("Samples/Value/FunctionCallOneParam.json");
        HandlerOutput node = valueHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<FunctionCallValue>());
        
        var funcCall = (FunctionCallValue)node.node!;
        Assert.That(funcCall.Identifier, Is.EqualTo("print"));
        Assert.That(funcCall.Parameters.Count, Is.EqualTo(1));
        Assert.That(funcCall.Parameters[0], Is.TypeOf<LiteralValue>());
        
        var literalParam = (LiteralValue)funcCall.Parameters[0];
        Assert.That(literalParam.Value, Is.EqualTo("42"));
    }

    [Test]
    public void FunctionCallMultipleParameters()
    {
        var output = LexerFileReader.ParseFile("Samples/Value/FunctionCallMultipleParams.json");
        HandlerOutput node = valueHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<FunctionCallValue>());
        
        var funcCall = (FunctionCallValue)node.node!;
        Assert.That(funcCall.Identifier, Is.EqualTo("print"));
        Assert.That(funcCall.Parameters.Count, Is.EqualTo(2));
        Assert.That(funcCall.Parameters[0], Is.TypeOf<LiteralValue>());
        Assert.That(funcCall.Parameters[1], Is.TypeOf<LiteralValue>());
    }

    [Test]
    public void FunctionCommaEnd()
    {
        var output = LexerFileReader.ParseFile("Samples/Value/Invalid/FunctionCallCommaEnd.json");
        Assert.Throws<SyntaxErrorException>(() => 
        {
            HandlerOutput node = valueHandler.HandleToken(output.Tokens, 0);
        });

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(1));
    }

    [Test]
    public void StructInitNoKeys()
    {
        var output = LexerFileReader.ParseFile("Samples/Value/StructInitNoKeys.json");
        HandlerOutput node = valueHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<StructInitialization>());
        
        var structInit = (StructInitialization)node.node!;
        Assert.That(structInit.Identifier, Is.EqualTo("structTest"));
        Assert.That(structInit.Keys.Count, Is.EqualTo(0));
    }

    [Test]
    public void StructInitOneKey()
    {
        var output = LexerFileReader.ParseFile("Samples/Value/StructInitOneKey.json");
        HandlerOutput node = valueHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<StructInitialization>());
        
        var structInit = (StructInitialization)node.node!;
        Assert.That(structInit.Identifier, Is.EqualTo("structTest"));
        Assert.That(structInit.Keys.Count, Is.EqualTo(1));
        Assert.That(structInit.Keys[0].Key, Is.EqualTo("x"));
        Assert.That(structInit.Keys[0].Value, Is.TypeOf<LiteralValue>());
    }

    [Test]
    public void StructInitMultipleKeys()
    {
        var output = LexerFileReader.ParseFile("Samples/Value/StructInitMultipleKeys.json");
        HandlerOutput node = valueHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<StructInitialization>());
        
        var structInit = (StructInitialization)node.node!;
        Assert.That(structInit.Identifier, Is.EqualTo("structTest"));
        Assert.That(structInit.Keys.Count, Is.EqualTo(2));
        Assert.That(structInit.Keys[0].Key, Is.EqualTo("value"));
        Assert.That(structInit.Keys[0].Value, Is.TypeOf<IdentifierValue>());
        Assert.That(structInit.Keys[1].Key, Is.EqualTo("param2"));
        Assert.That(structInit.Keys[1].Value, Is.TypeOf<LiteralValue>());
    }

    [Test]
    public void StructInitFunctionCallValue()
    {
        var output = LexerFileReader.ParseFile("Samples/Value/StructInitFunctionCall.json");
        HandlerOutput node = valueHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<StructInitialization>());
        
        var structInit = (StructInitialization)node.node!;
        Assert.That(structInit.Identifier, Is.EqualTo("ident"));
        Assert.That(structInit.Keys.Count, Is.EqualTo(2));

        Assert.That(structInit.Keys[0].Key, Is.EqualTo("key"));
        Assert.That(structInit.Keys[0].Value, Is.TypeOf<FunctionCallValue>());
        var funcValue = (FunctionCallValue)structInit.Keys[0].Value;
        Assert.That(funcValue.Identifier, Is.EqualTo("test"));
        Assert.That(funcValue.Parameters.Count, Is.EqualTo(2));
        Assert.That(funcValue.Parameters[0], Is.TypeOf<LiteralValue>());
        Assert.That(((LiteralValue)funcValue.Parameters[0]).Value, Is.EqualTo("3"));
        Assert.That(funcValue.Parameters[1], Is.TypeOf<IdentifierValue>());
        Assert.That(((IdentifierValue)funcValue.Parameters[1]).Value, Is.EqualTo("x"));

        Assert.That(structInit.Keys[1].Key, Is.EqualTo("pair"));
        Assert.That(structInit.Keys[1].Value, Is.TypeOf<LiteralValue>());
        var pairValue = (LiteralValue)structInit.Keys[1].Value;
        Assert.That(pairValue.Type.DataType, Is.EqualTo(DataTypes.DOUBLE));
        Assert.That(pairValue.Value, Is.EqualTo("5.5"));
    }

    [Test]
    public void StructInitUndefinedKey()
    {
        var output = LexerFileReader.ParseFile("Samples/Value/Invalid/StructInitUndefinedKey.json");
        valueHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(1));
    }

    [Test]
    public void StructInitTrailingComma()
    {
        var output = LexerFileReader.ParseFile("Samples/Value/Invalid/StructInitTrailingComma.json");
        Assert.Throws<SyntaxErrorException>(() => 
        {
            HandlerOutput node = valueHandler.HandleToken(output.Tokens, 0);
        });

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(1));
    }
}
