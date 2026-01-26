namespace SpeculaSyntaxAnalyzer.ParseTree;

public interface FuncDef : RootStatement;
public interface ParamNode : ParseNode;  // used for things that can be parameters such as self in interface
public record FuncParam(string Identifier, TypeDefinitionNode Definition) : ParamNode;
public record FuncParams(PrintableList<FuncParam> Params) : ParseNode; // Used for parsing parameter lists
// Used for reusable function shapes (param) {body}
public record FuncShapeNode(FuncParams Parameters, TypeDefinitionNode ReturnType, BodyNode Body) : ParseNode;
public record FuncDefNode(string Identifier, bool IsAsync, FuncShapeNode FunctionNode) : FuncDef, ListenerBody;
public record ThreadDefNode(string Identifier, FuncShapeNode FunctionNode) : FuncDef, ListenerBody;

public record SpawnThreadNode(FunctionCallValue ThreadFunction) : Expression;
public record AwaitNode(Expression AwaitedExpression) : Statement;