namespace SpeculaSyntaxAnalyzer.ParseTree;


public record TypeDefinitionNode(TypeNode DataType, Capabilities Capabilities) : ParseNode;