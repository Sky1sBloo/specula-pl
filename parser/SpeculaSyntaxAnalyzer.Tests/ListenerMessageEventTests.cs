using NUnit.Framework;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;
using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ListenerMessageEventTests
{
    private ListenerOnEventHandler onEventHandler = null!;
    private ErrorsHandler errorsHandler = null!;

    [SetUp]
    public void SetUp()
    {
        errorsHandler = new();
        onEventHandler = new(errorsHandler);
    }

    [Test]
    public void BasicCase()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/MessageEvents/BasicCase.json");
        HandlerOutput node = onEventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerMessageEventNode>());
        
        var messageEvent = (ListenerMessageEventNode)node.node!;
        Assert.That(messageEvent.Name, Is.EqualTo("MoveCommand"));
        Assert.That(messageEvent.Parameters.Params.Count, Is.EqualTo(2));
        Assert.That(messageEvent.Parameters.Params[0].Identifier, Is.EqualTo("direction"));
        Assert.That(messageEvent.Parameters.Params[1].Identifier, Is.EqualTo("speed"));
    }

    [Test]
    public void SingleParameter()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/MessageEvents/SingleParam.json");
        HandlerOutput node = onEventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerMessageEventNode>());
        
        var messageEvent = (ListenerMessageEventNode)node.node!;
        Assert.That(messageEvent.Name, Is.EqualTo("SetSpeed"));
        Assert.That(messageEvent.Parameters.Params.Count, Is.EqualTo(1));
        Assert.That(messageEvent.Parameters.Params[0].Identifier, Is.EqualTo("value"));
    }

    [Test]
    public void NoParameters()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/MessageEvents/NoParameters.json");
        HandlerOutput node = onEventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerMessageEventNode>());
        
        var messageEvent = (ListenerMessageEventNode)node.node!;
        Assert.That(messageEvent.Name, Is.EqualTo("StatusRequest"));
        Assert.That(messageEvent.Parameters.Params.Count, Is.EqualTo(0));
    }

    [Test]
    public void EmptyBody()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/MessageEvents/EmptyBody.json");
        HandlerOutput node = onEventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerMessageEventNode>());
        
        var messageEvent = (ListenerMessageEventNode)node.node!;
        foreach (var statement in messageEvent.Body.Statements)
        {
            Console.WriteLine(statement);
        }
        Assert.That(messageEvent.Body.Statements.Count, Is.EqualTo(0));
    }

    [Test]
    public void NoRespond()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/MessageEvents/NoRespond.json");
        HandlerOutput node = onEventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerMessageEventNode>());
        
        var messageEvent = (ListenerMessageEventNode)node.node!;
        Assert.That(messageEvent.Name, Is.EqualTo("LogCommand"));
        // Body should have statements but no respond
        Assert.That(messageEvent.Body.Statements.Count, Is.GreaterThan(0));
    }

    [Test]
    public void InvalidMissingName()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/MessageEvents/Invalid/NoName.json");
        Assert.Throws<SyntaxErrorException>(() => onEventHandler.HandleToken(output.Tokens, 0));
    }

    [Test]
    public void InvalidMissingBodyBraces()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/MessageEvents/Invalid/NoBodyBraces.json");
        HandlerOutput node = onEventHandler.HandleToken(output.Tokens, 0);
        // Should have errors
        Assert.That(errorsHandler.ErrorList.Count, Is.GreaterThan(0));
    }

    [Test]
    public void InvalidMissingParameterBraces()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/MessageEvents/Invalid/NoParamBraces.json");
        HandlerOutput node = onEventHandler.HandleToken(output.Tokens, 0);
        // Should have errors
        Assert.That(errorsHandler.ErrorList.Count, Is.GreaterThan(0));
    }

    [Test]
    public void InvalidTrailingComma()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/MessageEvents/Invalid/TrailingComma.json");
        HandlerOutput node = onEventHandler.HandleToken(output.Tokens, 0);
        // Should have errors
        Assert.That(errorsHandler.ErrorList.Count, Is.GreaterThan(0));
    }
}
