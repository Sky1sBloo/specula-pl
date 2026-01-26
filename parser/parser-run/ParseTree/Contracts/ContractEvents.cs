namespace SpeculaSyntaxAnalyzer.ParseTree;

public interface ContractEventNode : ParseNode;
public record ContractAutoResetEventNode(PrintableList<StateNode> States) : ContractEventNode;
public record ContractAutoMoveEventNode(PrintableList<StateNode> States, StateNode TargetState) : ContractEventNode;
public record ContractFailEventNode(string Identifier, FuncParams Parameters) : ContractEventNode;