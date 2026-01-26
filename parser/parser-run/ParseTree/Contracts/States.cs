namespace SpeculaSyntaxAnalyzer.ParseTree;

public record InitStateNode(StateNode State) : StateType;
public record StateTransitionsNode(PrintableList<StateTransition> Transitions) : ParseNode;
public interface StateType : ParseNode;
public interface StateTransition : StateType;
public record StateNode(string Name) : StateType;
public record StateTransitionNode(StateNode State, StateType TargetState) : StateTransition;
public record StateTransitionBidirectionalNode(StateNode StateA, StateType StateB) : StateTransition;