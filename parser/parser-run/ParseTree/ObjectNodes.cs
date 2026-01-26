namespace SpeculaSyntaxAnalyzer.ParseTree;

public record StructField (string Identifier, TypeDefinitionNode Definition) : ParseNode;
public record StructDefNode (string StructName, PrintableList<StructField> Fields) : RootStatement;
public record InterfaceSelfParam(string Identifier) : ParamNode;  // used for self parameter in interface methods

public interface InterfaceFuncNode : ParseNode; 
public record InterfaceFuncReturnSelfNode(string Identifier, PrintableList<ParamNode> Parameters) : InterfaceFuncNode;
public record InterfaceFuncReturnNode (string Identifier, PrintableList<ParamNode> Parameters, TypeDefinitionNode ReturnType) : InterfaceFuncNode;
public record InterfaceDefNode (string InterfaceName, PrintableList<InterfaceFuncNode> Methods) : RootStatement;

public record ImplDefNode (string StructName, string InterfaceName, PrintableList<FuncDef> Methods) : RootStatement;