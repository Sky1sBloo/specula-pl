using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ContractStateTransitionsTests
{
    private ContractStateTransitionsHandler stateTransitionsHandler;
    private ErrorsHandler errorsHandler;

    [SetUp]
    public void Setup()
    {
        errorsHandler = new();
        stateTransitionsHandler = new(errorsHandler);
    }

    [Test]
    public void SimpleDirectionalTransition()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/States/SimpleDirectional.json");
        HandlerOutput node = stateTransitionsHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<StateTransitionsNode>());
        
        var transitions = (StateTransitionsNode)node.node!;
        Assert.That(transitions.Transitions.Count, Is.EqualTo(1));
        Assert.That(transitions.Transitions[0], Is.TypeOf<StateTransitionNode>());
        
        var transition = (StateTransitionNode)transitions.Transitions[0];
        Assert.That(transition.State.Name, Is.EqualTo("CommandSent"));
        Assert.That(((StateNode)transition.TargetState).Name, Is.EqualTo("Idle"));
    }

    [Test]
    public void BidirectionalTransition()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/States/Chain.json");
        HandlerOutput node = stateTransitionsHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<StateTransitionsNode>());
        
        var transitions = (StateTransitionsNode)node.node!;
        Assert.That(transitions.Transitions.Count, Is.GreaterThan(0));
        
        // First transition should be bidirectional
        Assert.That(transitions.Transitions[0], Is.TypeOf<StateTransitionBidirectionalNode>());
        var bidirectional = (StateTransitionBidirectionalNode)transitions.Transitions[0];
        Assert.That(bidirectional.StateA.Name, Is.EqualTo("Idle"));
        Assert.That(((StateNode)bidirectional.StateB).Name, Is.EqualTo("CommandSent"));
    }

    [Test]
    public void ChainedTransitions()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/States/Chain.json");
        HandlerOutput node = stateTransitionsHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<StateTransitionsNode>());
        
        var transitions = (StateTransitionsNode)node.node!;
        // Chain should have: Idle <-> CommandSent -> Acknowledged -> Completed
        Assert.That(transitions.Transitions.Count, Is.EqualTo(3));
        
        // Transition 1: Idle <-> CommandSent (bidirectional)
        Assert.That(transitions.Transitions[0], Is.TypeOf<StateTransitionBidirectionalNode>());
        
        // Transition 2: CommandSent -> Acknowledged (directional)
        Assert.That(transitions.Transitions[1], Is.TypeOf<StateTransitionNode>());
        var trans2 = (StateTransitionNode)transitions.Transitions[1];
        Assert.That(trans2.State.Name, Is.EqualTo("CommandSent"));
        Assert.That(((StateNode)trans2.TargetState).Name, Is.EqualTo("Acknowledged"));
        
        // Transition 3: Acknowledged -> Completed (directional)
        Assert.That(transitions.Transitions[2], Is.TypeOf<StateTransitionNode>());
        var trans3 = (StateTransitionNode)transitions.Transitions[2];
        Assert.That(trans3.State.Name, Is.EqualTo("Acknowledged"));
        Assert.That(((StateNode)trans3.TargetState).Name, Is.EqualTo("Completed"));
    }

    [Test]
    public void MultipleStateDeclarations()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/States/Multiple.json");
        HandlerOutput node = stateTransitionsHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<StateTransitionsNode>());
        
        var transitions = (StateTransitionsNode)node.node!;
        Assert.That(transitions.Transitions.Count, Is.GreaterThanOrEqualTo(3));
    }

    [Test]
    public void NoTransitions()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/States/NoTransition.json");
        HandlerOutput node = stateTransitionsHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<StateTransitionsNode>());
        var transitions = (StateTransitionsNode)node.node!;
        Assert.That(transitions.Transitions.Count, Is.EqualTo(0));
    }
}
