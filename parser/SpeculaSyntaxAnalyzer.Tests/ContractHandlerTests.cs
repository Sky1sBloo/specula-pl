using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ContractHandlerTests
{
    private ContractHandler contractHandler;
    private ErrorsHandler errorsHandler;

    [SetUp]
    public void Setup()
    {
        errorsHandler = new();
        contractHandler = new(errorsHandler);
    }

    [Test]
    public void MinimalContract()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/ContractDefinition/Minimal.json");
        HandlerOutput result = contractHandler.HandleToken(output.Tokens, 0);

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(result.node, Is.TypeOf<ContractNode>());

        var contract = (ContractNode)result.node!;
        Assert.That(contract.Name, Is.EqualTo("Minimal"));
        Assert.That(contract.InitState?.State.Name, Is.EqualTo("Idle"));
        Assert.That(contract.Roles?.Roles.Count, Is.EqualTo(1));
        Assert.That(contract.Roles?.Roles[0].Name, Is.EqualTo("Robot"));
        Assert.That(contract.StateTransitions.Count, Is.EqualTo(0));
        Assert.That(contract.MessageEvents.Count, Is.EqualTo(0));
        Assert.That(contract.Events.Count, Is.EqualTo(0));
    }

    [Test]
    public void EmptySections()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/ContractDefinition/EmptySections.json");
        HandlerOutput result = contractHandler.HandleToken(output.Tokens, 0);

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(result.node, Is.TypeOf<ContractNode>());

        var contract = (ContractNode)result.node!;
        Assert.That(contract.Name, Is.EqualTo("EmptySections"));
        Assert.That(contract.InitState?.State.Name, Is.EqualTo("Idle"));
        Assert.That(contract.Roles?.Roles.Count, Is.EqualTo(2));
        Assert.That(contract.StateTransitions.Count, Is.EqualTo(1));
        Assert.That(contract.MessageEvents.Count, Is.EqualTo(0));
        Assert.That(contract.Events.Count, Is.EqualTo(0));
    }

    [Test]
    public void FullDefinition_UsesBrackets()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/ContractDefinition/FullDefinition.json");
        HandlerOutput result = contractHandler.HandleToken(output.Tokens, 0);

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(result.node, Is.TypeOf<ContractNode>());
    }

    [Test]
    public void Invalid_MissingInitState()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/ContractDefinition/Invalid/NoInitState.json");
        HandlerOutput result = contractHandler.HandleToken(output.Tokens, 0);

        Assert.That(result.node, Is.Null);
        Assert.That(errorsHandler.ErrorList.Count, Is.GreaterThan(0));
    }

    [Test]
    public void Invalid_MissingRoles()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/ContractDefinition/Invalid/NoRoles.json");
        HandlerOutput result = contractHandler.HandleToken(output.Tokens, 0);

        Assert.That(result.node, Is.Null);
        Assert.That(errorsHandler.ErrorList.Count, Is.GreaterThan(0));
    }

    [Test]
    public void Invalid_OrphanMessageEvent_NoDirection()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/ContractDefinition/Invalid/OrphanMessageEvent.json");
        HandlerOutput result = contractHandler.HandleToken(output.Tokens, 0);

        Assert.That(result.node, Is.Null);
        Assert.That(errorsHandler.ErrorList.Count, Is.GreaterThan(0));
    }

    [Test]
    public void Invalid_DirectionBracketsWithoutEvents()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/ContractDefinition/Invalid/DirectionBracketsNoMessageEvents.json");
        HandlerOutput result = contractHandler.HandleToken(output.Tokens, 0);

        Assert.That(result.node, Is.Null);
        Assert.That(errorsHandler.ErrorList.Count, Is.GreaterThan(0));
    }
}
