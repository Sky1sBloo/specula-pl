using NUnit.Framework;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;
using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ListenerFailEventTests
{
    private ListenerFailEventHandler failEventHandler = null!;
    private ErrorsHandler errorsHandler = null!;

    [SetUp]
    public void SetUp()
    {
        errorsHandler = new();
        failEventHandler = new(errorsHandler);
    }

    [Test]
    public void FailEventWithoutParameters()
    {
        // on fail low_battery {
        //     stop();
        //     respond Error();
        // }
        var output = LexerFileReader.ParseFile("Samples/Listener/FailEvents/NoParam.json");
        // Skip the ON token to start at K_FAIL
        HandlerOutput node = failEventHandler.HandleToken(output.Tokens, 1);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerFailEventNode>());
        
        var failEvent = (ListenerFailEventNode)node.node!;
        Assert.That(failEvent.Name, Is.EqualTo("low_battery"));
        Assert.That(failEvent.Parameters.Params.Count, Is.EqualTo(0));
        Assert.That(failEvent.Body.Statements.Count, Is.GreaterThan(0));
    }

    [Test]
    public void FailEventWithParameters()
    {
        // on fail connection_lost(timeout: int, retries: int) {
        //     log("Connection lost");
        //     fail critical();
        // }
        var output = LexerFileReader.ParseFile("Samples/Listener/FailEvents/WithParam.json");
        HandlerOutput node = failEventHandler.HandleToken(output.Tokens, 1);
        Assert.That(node.node, Is.TypeOf<ListenerFailEventNode>());
        
        var failEvent = (ListenerFailEventNode)node.node!;
        Assert.That(failEvent.Name, Is.EqualTo("connection_lost"));
        Assert.That(failEvent.Parameters.Params.Count, Is.EqualTo(2));
        Assert.That(failEvent.Parameters.Params[0].Identifier, Is.EqualTo("timeout"));
        Assert.That(failEvent.Parameters.Params[1].Identifier, Is.EqualTo("retries"));
    }

    [Test]
    public void FailEventWithSingleParameter()
    {
        // on fail invalid_command(code: int) {
        //     respond ErrorCode(code);
        // }
        var output = LexerFileReader.ParseFile("Samples/Listener/FailEvents/SingleParam.json");
        HandlerOutput node = failEventHandler.HandleToken(output.Tokens, 1);
        Assert.That(node.node, Is.TypeOf<ListenerFailEventNode>());
        
        var failEvent = (ListenerFailEventNode)node.node!;
        Assert.That(failEvent.Name, Is.EqualTo("invalid_command"));
        Assert.That(failEvent.Parameters.Params.Count, Is.EqualTo(1));
        Assert.That(failEvent.Parameters.Params[0].Identifier, Is.EqualTo("code"));
    }

    [Test]
    public void MinimalFailEvent()
    {
        // on fail x{}
        var output = LexerFileReader.ParseFile("Samples/Listener/FailEvents/Minimal.json");
        HandlerOutput node = failEventHandler.HandleToken(output.Tokens, 1);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerFailEventNode>());
        
        var failEvent = (ListenerFailEventNode)node.node!;
        Assert.That(failEvent.Name, Is.EqualTo("x"));
        Assert.That(failEvent.Parameters.Params.Count, Is.EqualTo(0));
        Assert.That(failEvent.Body.Statements.Count, Is.EqualTo(0));
    }

    [Test]
    public void FailEventWithMultipleFailStatements()
    {
        // on fail error {
        //     fail sub_error();
        //     fail another_error(123);
        // }
        var output = LexerFileReader.ParseFile("Samples/Listener/FailEvents/MultiFailStatements.json");
        HandlerOutput node = failEventHandler.HandleToken(output.Tokens, 1);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerFailEventNode>());
        
        var failEvent = (ListenerFailEventNode)node.node!;
        Assert.That(failEvent.Name, Is.EqualTo("error"));
        Assert.That(failEvent.Body.Statements.Count, Is.EqualTo(2));
    }

    [Test]
    public void FailEventWithNestedControlFlow()
    {
        // on fail overload(current: int) {
        //     if (current > 100) {
        //         shutdown();
        //     }
        // }
        var output = LexerFileReader.ParseFile("Samples/Listener/FailEvents/NestedControlFlow.json");
        HandlerOutput node = failEventHandler.HandleToken(output.Tokens, 1);
        Assert.That(node.node, Is.TypeOf<ListenerFailEventNode>());
        
        var failEvent = (ListenerFailEventNode)node.node!;
        Assert.That(failEvent.Name, Is.EqualTo("overload"));
        Assert.That(failEvent.Parameters.Params.Count, Is.EqualTo(1));
        Assert.That(failEvent.Body.Statements.Count, Is.GreaterThan(0));
    }

    [Test]
    public void InvalidFailEventTrailingComma()
    {
        // on fail error(code: int, message: string,) { log(message); }
        // Trailing comma in parameter list should be caught by delegated FuncParamsHandler
        // This adds to error list instead of throwing
        var output = LexerFileReader.ParseFile("Samples/Listener/FailEvents/Invalid/TrailingComma.json");
        HandlerOutput node = failEventHandler.HandleToken(output.Tokens, 1);
        Assert.That(errorsHandler.ErrorList.Count, Is.GreaterThan(0));
    }
}
