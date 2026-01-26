using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ContractEventHandlerTests
{
    private ContractEventHandler eventHandler;
    private ErrorsHandler errorsHandler;

    [SetUp]
    public void Setup()
    {
        errorsHandler = new();
        eventHandler = new(errorsHandler);
    }

    [Test]
    public void FailEvent()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Events/FailEvent.json");
        HandlerOutput node = eventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ContractFailEventNode>());
        
        var failEvent = (ContractFailEventNode)node.node!;
        Assert.That(failEvent.Identifier, Is.EqualTo("low_battery"));
        Assert.That(failEvent.Parameters.Params.Count, Is.EqualTo(0));
    }

    [Test]
    public void AutoResetEvent()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Events/AutoResetEvent.json");
        HandlerOutput node = eventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ContractAutoResetEventNode>());
        
        var resetEvent = (ContractAutoResetEventNode)node.node!;
        Assert.That(resetEvent.States.Count, Is.EqualTo(1));
        Assert.That(resetEvent.States[0].Name, Is.EqualTo("Completed"));
    }

    [Test]
    public void AutoMoveEvent()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Events/AutoMoveEvent.json");
        HandlerOutput node = eventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ContractAutoMoveEventNode>());
        
        var moveEvent = (ContractAutoMoveEventNode)node.node!;
        Assert.That(moveEvent.States.Count, Is.EqualTo(1));
        Assert.That(moveEvent.States[0].Name, Is.EqualTo("Completed"));
        Assert.That(moveEvent.TargetState.Name, Is.EqualTo("Idle"));
    }

    [Test]
    public void InvalidMissingSemiColon()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Events/Invalid/MissingSemiColon.json");
        Assert.Throws<SyntaxErrorException>(() => eventHandler.HandleToken(output.Tokens, 0),
            "Should throw error for missing semicolon");
    }

    [Test]
    public void InvalidMissingStateAfter()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Events/Invalid/MissingStateAfter.json");
        Assert.Throws<SyntaxErrorException>(() => eventHandler.HandleToken(output.Tokens, 0),
            "Should throw error for missing state after 'after' keyword");
    }

    [Test]
    public void InvalidMissingToAutoMove()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Events/Invalid/MissingToAutoMove.json");
        Assert.Throws<SyntaxErrorException>(() => eventHandler.HandleToken(output.Tokens, 0),
            "Should throw error for missing 'to' keyword in auto-move");
    }

    [Test]
    public void InvalidMissingTargetState()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Events/Invalid/MissingTargetState.json");
        Assert.Throws<SyntaxErrorException>(() => eventHandler.HandleToken(output.Tokens, 0),
            "Should throw error for missing target state in auto-move");
    }

    [Test]
    public void InvalidNoAfter()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Events/Invalid/NoAfter.json");
        Assert.Throws<SyntaxErrorException>(() => eventHandler.HandleToken(output.Tokens, 0),
            "Should throw error for missing 'after' keyword");
    }

    [Test]
    public void InvalidActionType()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Events/Invalid/InvalidAction.json");
        Assert.Throws<SyntaxErrorException>(() => eventHandler.HandleToken(output.Tokens, 0),
            "Should throw error for invalid action type");
    }

    [Test]
    public void InvalidEmptyActionName()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Events/Invalid/EmptyActionName.json");
        Assert.Throws<SyntaxErrorException>(() => eventHandler.HandleToken(output.Tokens, 0),
            "Should throw error for empty action name");
    }
}
