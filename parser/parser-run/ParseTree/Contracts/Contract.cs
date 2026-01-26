namespace SpeculaSyntaxAnalyzer.ParseTree;

public record ContractNode(string Name,
    InitStateNode InitState, 
    RolesNode Roles,
    PrintableList<StateTransitionsNode> StateTransitions,
    PrintableList<ContractMessageEventsNode> MessageEvents,
    PrintableList<ContractEventNode> Events
    ) : RootStatement;