namespace SpeculaSyntaxAnalyzer.ParseTree;

public record ListenerNode(RoleNode Role, 
    string ContractName, 
    ValueNode Target, 
    PrintableList<ListenerBody> Body) : RootStatement;

public interface ListenerBody : ParseNode;