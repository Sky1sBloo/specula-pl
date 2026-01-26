using NUnit.Framework;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;
using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ListenerEventTests
{
    private ListenerEventHandler eventHandler = null!;
    private ErrorsHandler errorsHandler = null!;

    [SetUp]
    public void SetUp()
    {
        errorsHandler = new();
        eventHandler = new(errorsHandler);
    }

    // ===== Valid Event Tests (after/before) =====

    [Test]
    public void BasicAfterEvent()
    {
        // after Acknowledged {
        //     let pos = getPosition();
        //     respond Status(pos, battery);
        // }
        var output = LexerFileReader.ParseFile("Samples/Listener/Events/BasicAfterEvent.json");
        HandlerOutput node = eventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerEventNode>());
        
        var eventNode = (ListenerEventNode)node.node!;
        Assert.That(eventNode.Name, Is.EqualTo("Acknowledged"));
        Assert.That(eventNode.EventType, Is.EqualTo(ListenerEventType.AFTER));
        Assert.That(eventNode.Body.Statements.Count, Is.GreaterThan(0));
    }

    [Test]
    public void BasicBeforeEvent()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/Events/BasicBeforeEvent.json");
        HandlerOutput node = eventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerEventNode>());
        
        var eventNode = (ListenerEventNode)node.node!;
        Assert.That(eventNode.Name, Is.EqualTo("SendCommand"));
        Assert.That(eventNode.EventType, Is.EqualTo(ListenerEventType.BEFORE));
    }

    [Test]
    public void EventWithEmptyBody()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/Events/EmptyBody.json");
        HandlerOutput node = eventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerEventNode>());
        
        var eventNode = (ListenerEventNode)node.node!;
        Assert.That(eventNode.Name, Is.EqualTo("Done"));
        Assert.That(eventNode.Body.Statements.Count, Is.EqualTo(0));
    }

    [Test]
    public void MinimalEvent()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/Events/Minimal.json");
        HandlerOutput node = eventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerEventNode>());
        
        var eventNode = (ListenerEventNode)node.node!;
        Assert.That(eventNode.Name, Is.EqualTo("X"));
    }

    [Test]
    public void EventWithNestedControlFlow()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/Events/NestedControlFlow.json");
        HandlerOutput node = eventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerEventNode>());
    }

    [Test]
    public void EventWithBothRespondAndFail()
    {
        // Event containing both respond and fail statements
        var output = LexerFileReader.ParseFile("Samples/Listener/Events/BothRespondAndFail.json");
        HandlerOutput node = eventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerEventNode>());
    }

    [Test]
    public void InvalidEventWithParameters()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/Events/Invalid/HasParam.json");
        HandlerOutput node = eventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.GreaterThan(0));
    }

    [Test]
    public void InvalidEventMissingName()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/Events/Invalid/NoName.json");
        Assert.Throws<SyntaxErrorException>(() => eventHandler.HandleToken(output.Tokens, 0));
    }

    [Test]
    public void InvalidEventMissingBody()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/Events/Invalid/NoBody.json");
        Assert.Throws<SyntaxErrorException>(() => eventHandler.HandleToken(output.Tokens, 0));
    }
}
