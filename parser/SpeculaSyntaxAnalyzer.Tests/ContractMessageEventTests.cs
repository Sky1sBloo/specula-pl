using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ContractMessageEventTests
{
    private ContractMessageEventHandler messageEventHandler;
    private ErrorsHandler errorsHandler;

    [SetUp]
    public void Setup()
    {
        errorsHandler = new();
        messageEventHandler = new(errorsHandler);
    }

    [Test]
    public void SimpleMessage()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEvents/SimpleMessage.json");
        HandlerOutput node = messageEventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ContractMessageEventNode>());
        
        var messageEvent = (ContractMessageEventNode)node.node!;
        Assert.That(messageEvent.Name, Is.EqualTo("MoveCommand"));
        Assert.That(messageEvent.Parameters.Params.Count, Is.EqualTo(2));
        Assert.That(messageEvent.InitialState.Name, Is.EqualTo("Idle"));
        Assert.That(messageEvent.NextState.Name, Is.EqualTo("CommandSent"));
    }

    [Test]
    public void MessageWithMultipleFields()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEvents/MessageWithFields.json");
        HandlerOutput node = messageEventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ContractMessageEventNode>());
        
        var messageEvent = (ContractMessageEventNode)node.node!;
        Assert.That(messageEvent.Name, Is.EqualTo("UpdateStatus"));
        Assert.That(messageEvent.Parameters.Params.Count, Is.EqualTo(4));
        Assert.That(messageEvent.InitialState.Name, Is.EqualTo("Acknowledged"));
        Assert.That(messageEvent.NextState.Name, Is.EqualTo("Completed"));
    }

    [Test]
    public void MessageWithNoFields()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEvents/NoFields.json");
        HandlerOutput node = messageEventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<ContractMessageEventNode>());
        
        var messageEvent = (ContractMessageEventNode)node.node!;
        Assert.That(messageEvent.Name, Is.EqualTo("Heartbeat"));
        Assert.That(messageEvent.Parameters.Params.Count, Is.EqualTo(0));
        Assert.That(messageEvent.InitialState.Name, Is.EqualTo("Idle"));
        Assert.That(messageEvent.NextState.Name, Is.EqualTo("Idle"));
    }

    [Test]
    public void InvalidMissingBraces()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEvents/Invalid/MissingBraces.json");
        Assert.Throws<SyntaxErrorException>(() => messageEventHandler.HandleToken(output.Tokens, 0),
            "Should throw error for missing braces");
    }

    [Test]
    public void InvalidNoAtSymbol()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEvents/Invalid/NoAtSymbol.json");
        Assert.Throws<SyntaxErrorException>(() => messageEventHandler.HandleToken(output.Tokens, 0),
            "Should throw error for missing @ symbol");
    }

    [Test]
    public void InvalidNoTransition()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEvents/Invalid/NoTransition.json");
        Assert.Throws<SyntaxErrorException>(() => messageEventHandler.HandleToken(output.Tokens, 0),
            "Should throw error for missing transition");
    }

    [Test]
    public void InvalidNoTargetState()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEvents/Invalid/NoTargetState.json");
        Assert.Throws<SyntaxErrorException>(() => messageEventHandler.HandleToken(output.Tokens, 0),
            "Should throw error for missing target state");
    }

    [Test]
    public void InvalidMalformedFieldDeclaration()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEvents/Invalid/MalformedFieldDeclaration.json");
        Assert.Throws<SyntaxErrorException>(() => messageEventHandler.HandleToken(output.Tokens, 0),
            "Should throw error for malformed field");
    }

    [Test]
    public void InvalidExtraSymbols()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEvents/Invalid/ExtraSymbols.json");
        Assert.Throws<SyntaxErrorException>(() => messageEventHandler.HandleToken(output.Tokens, 0),
            "Should throw error for extra symbols");
    }

    [Test]
    public void InvalidBidirectionalTransition()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEvents/Invalid/BidirectionalTransition.json");
        Assert.Throws<SyntaxErrorException>(() => messageEventHandler.HandleToken(output.Tokens, 0),
            "Should throw error for bidirectional transition (not allowed for messages)");
    }

    [Test]
    public void ValidFieldTypes()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/MessageEvents/MessageWithFields.json");
        HandlerOutput node = messageEventHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        
        var messageEvent = (ContractMessageEventNode)node.node!;
        Assert.That(messageEvent.Parameters.Params[0].Identifier, Is.EqualTo("code"));    // code: int
        Assert.That(messageEvent.Parameters.Params[1].Identifier, Is.EqualTo("message"));    // message: str
        Assert.That(messageEvent.Parameters.Params[2].Identifier, Is.EqualTo("timestamp"));  // timestamp: float
    }
}
