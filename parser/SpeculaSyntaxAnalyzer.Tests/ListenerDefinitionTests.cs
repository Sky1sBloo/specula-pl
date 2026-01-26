using NUnit.Framework;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;
using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ListenerDefinitionTests
{
    private ListenerHandler listenerHandler = null!;
    private ErrorsHandler errorsHandler = null!;

    [SetUp]
    public void SetUp()
    {
        errorsHandler = new();
        listenerHandler = new(errorsHandler);
    }

    // VALID TEST CASES

    [Test]
    public void BasicListenerWithMessageEvent()
    {
        // listener target("0.0.0.0:6000") using MotionControl as Robot {
        //     on MoveCommand { direction, speed } {
        //         moveMotors(direction, speed);
        //         respond Ack(true);
        //     }
        // }
        var output = LexerFileReader.ParseFile("Samples/Listener/Definition/Basic.json");
        HandlerOutput node = listenerHandler.HandleToken(output.Tokens, 0);
        
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerNode>());
        
        var listener = (ListenerNode)node.node!;
        Assert.That(listener.Role.Name, Is.EqualTo("Robot"));
        Assert.That(listener.ContractName, Is.EqualTo("MotionControl"));
        Assert.That(listener.Body.Count, Is.EqualTo(1));
        Assert.That(listener.Body[0], Is.TypeOf<ListenerMessageEventNode>());
    }

    [Test]
    public void ListenerWithMessageEventNoParameters()
    {
        // listener target("localhost:8080") using EventHandler as Handler {
        //     on StartRequest { } {
        //         print("Starting");
        //         respond Ok();
        //     }
        // }
        var output = LexerFileReader.ParseFile("Samples/Listener/Definition/MessageEventNoParams.json");
        HandlerOutput node = listenerHandler.HandleToken(output.Tokens, 0);
        
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerNode>());
        
        var listener = (ListenerNode)node.node!;
        Assert.That(listener.Role.Name, Is.EqualTo("Handler"));
        Assert.That(listener.ContractName, Is.EqualTo("EventHandler"));
        Assert.That(listener.Body.Count, Is.EqualTo(1));
        
        var msgEvent = (ListenerMessageEventNode)listener.Body[0];
        Assert.That(msgEvent.Name, Is.EqualTo("StartRequest"));
        Assert.That(msgEvent.Parameters.Params.Count, Is.EqualTo(0));
    }

    [Test]
    public void ListenerWithBeforeAndAfterEvents()
    {
        // listener target("127.0.0.1:3000") using Monitor as Watcher {
        //     before Initialize {
        //         print("Before init");
        //     }
        //     after Initialize {
        //         print("After init");
        //     }
        // }
        var output = LexerFileReader.ParseFile("Samples/Listener/Definition/BeforeAfter.json");
        HandlerOutput node = listenerHandler.HandleToken(output.Tokens, 0);
        
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerNode>());
        
        var listener = (ListenerNode)node.node!;
        Assert.That(listener.Role.Name, Is.EqualTo("Watcher"));
        Assert.That(listener.Body.Count, Is.EqualTo(2));
        
        Assert.That(listener.Body[0], Is.TypeOf<ListenerEventNode>());
        var beforeEvent = (ListenerEventNode)listener.Body[0];
        Assert.That(beforeEvent.EventType, Is.EqualTo(ListenerEventType.BEFORE));
        Assert.That(beforeEvent.Name, Is.EqualTo("Initialize"));
        
        Assert.That(listener.Body[1], Is.TypeOf<ListenerEventNode>());
        var afterEvent = (ListenerEventNode)listener.Body[1];
        Assert.That(afterEvent.EventType, Is.EqualTo(ListenerEventType.AFTER));
        Assert.That(afterEvent.Name, Is.EqualTo("Initialize"));
    }

    [Test]
    public void ListenerWithFailEventWithParameters()
    {
        // listener target("0.0.0.0:9000") using ErrorHandler as ErrHandler {
        //     on fail connection_lost(timeout: int, retries: int) {
        //         print("Connection lost after", retries, "retries");
        //         alert("Error timeout:", timeout);
        //     }
        // }
        var output = LexerFileReader.ParseFile("Samples/Listener/Definition/FailEventWithParams.json");
        HandlerOutput node = listenerHandler.HandleToken(output.Tokens, 0);
        
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerNode>());
        
        var listener = (ListenerNode)node.node!;
        Assert.That(listener.Role.Name, Is.EqualTo("ErrHandler"));
        Assert.That(listener.Body.Count, Is.EqualTo(1));
        
        Assert.That(listener.Body[0], Is.TypeOf<ListenerFailEventNode>());
        var failEvent = (ListenerFailEventNode)listener.Body[0];
        Assert.That(failEvent.Name, Is.EqualTo("connection_lost"));
        Assert.That(failEvent.Parameters.Params.Count, Is.EqualTo(2));
        Assert.That(failEvent.Parameters.Params[0].Identifier, Is.EqualTo("timeout"));
        Assert.That(failEvent.Parameters.Params[1].Identifier, Is.EqualTo("retries"));
    }

    [Test]
    public void ListenerWithFunctionDefinition()
    {
        var output = LexerFileReader.ParseFile("Samples/Listener/Definition/WithFunctionDef.json");
        HandlerOutput node = listenerHandler.HandleToken(output.Tokens, 0);
        
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ListenerNode>());
        
        var listener = (ListenerNode)node.node!;
        Assert.That(listener.Role.Name, Is.EqualTo("Server"));
        Assert.That(listener.ContractName, Is.EqualTo("MotionControl"));
        Assert.That(listener.Body.Count, Is.EqualTo(1));
        Assert.That(listener.Body[0], Is.TypeOf<FuncDefNode>());
        
        var funcDef = (FuncDefNode)listener.Body[0];
        Assert.That(funcDef.Identifier, Is.EqualTo("test"));
    }

    // INVALID TEST CASES

    [Test]
    public void InvalidListenerMissingTarget()
    {
        // listener using MotionControl as Robot {
        //     on MoveCommand { direction } {
        //         respond Ack();
        //     }
        // }
        var output = LexerFileReader.ParseFile("Samples/Listener/Definition/Invalid/MissingTarget.json");
        Assert.Throws<SyntaxErrorException>(() => listenerHandler.HandleToken(output.Tokens, 0));
    }

    [Test]
    public void InvalidListenerMissingUsing()
    {
        // listener target("0.0.0.0:6000") MotionControl as Robot {
        //     on MoveCommand { } {
        //         respond Ack();
        //     }
        // }
        var output = LexerFileReader.ParseFile("Samples/Listener/Definition/Invalid/MissingUsing.json");
        Assert.Throws<SyntaxErrorException>(() => listenerHandler.HandleToken(output.Tokens, 0));
    }
}
