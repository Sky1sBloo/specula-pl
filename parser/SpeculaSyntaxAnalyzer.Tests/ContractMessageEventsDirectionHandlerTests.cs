using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ContractMessageEventsDirectionHandlerTests
{
    private ContractMessageEventsDirectionHandler directionHandler;
    private ErrorsHandler errorsHandler;

    [SetUp]
    public void Setup()
    {
        errorsHandler = new();
        directionHandler = new(errorsHandler);
    }

    [Test]
    public void SingleMessageEvent()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEventsDirection/SingleMessageEvent.json");
        HandlerOutput node = directionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ContractMessageEventsNode>());
        
        var events = (ContractMessageEventsNode)node.node!;
        Assert.That(events.From.Name, Is.EqualTo("Controller"));
        Assert.That(events.To.Name, Is.EqualTo("Robot"));
        Assert.That(events.Events.Count, Is.EqualTo(1));
        Assert.That(events.Events[0].Name, Is.EqualTo("MoveCommand"));
    }

    [Test]
    public void MultipleEvents()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEventsDirection/MultipleEvent.json");
        HandlerOutput node = directionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ContractMessageEventsNode>());
        
        var events = (ContractMessageEventsNode)node.node!;
        Assert.That(events.From.Name, Is.EqualTo("Controller"));
        Assert.That(events.To.Name, Is.EqualTo("Robot"));
        Assert.That(events.Events.Count, Is.GreaterThan(1));
    }

    [Test]
    public void InvalidEmptyBrackets()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEventsDirection/Invalid/EmptyBrackets.json");
        Assert.Throws<SyntaxErrorException>(() => directionHandler.HandleToken(output.Tokens, 0),
            "Should throw error for empty brackets");
    }

    [Test]
    public void InvalidMissingCloseBracket()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEventsDirection/Invalid/MissingCloseBracket.json");
        Assert.Throws<SyntaxErrorException>(() => directionHandler.HandleToken(output.Tokens, 0),
            "Should throw error for missing close bracket");
    }

    [Test]
    public void InvalidMissingFirstRole()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEventsDirection/Invalid/MissingFirstRole.json");
        Assert.Throws<SyntaxErrorException>(() => directionHandler.HandleToken(output.Tokens, 0),
            "Should throw error for missing first role");
    }

    [Test]
    public void InvalidMissingSecondRole()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEventsDirection/Invalid/MissingSecondRole.json");
        Assert.Throws<SyntaxErrorException>(() => directionHandler.HandleToken(output.Tokens, 0),
            "Should throw error for missing second role");
    }

    [Test]
    public void InvalidMessageEvent()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEventsDirection/Invalid/InvalidMessageEvent.json");
        directionHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.GreaterThan(0),
            "Should add error for invalid message event");
    }

    [Test]
    public void InvalidNoMessageEvent()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEventsDirection/Invalid/NoMessageEvent.json");
        Assert.Throws<SyntaxErrorException>(() => directionHandler.HandleToken(output.Tokens, 0),
            "Should throw error for no message event after direction");
    }
}
