namespace SpeculaSyntaxAnalyzer.ParseTree;

public record ContractMessageEventsNode(RoleNode From, RoleNode To, PrintableList<ContractMessageEventNode> Events) : ParseNode;
public record ContractMessageEventNode(string Name, FuncParams Parameters, StateNode InitialState, StateNode NextState) : ParseNode;