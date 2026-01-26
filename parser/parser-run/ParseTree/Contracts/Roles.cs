namespace SpeculaSyntaxAnalyzer.ParseTree;

public record RoleNode(string Name) : ParseNode;
public record RolesNode(PrintableList<RoleNode> Roles) : ParseNode;