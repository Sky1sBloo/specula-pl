namespace SpeculaSyntaxAnalyzer.ParseTree;

public interface IfNode : ParseNode;

public record ConditionalStatement(IfStatementNode IfStatement, PrintableList<ElseIfStatementNode> ElseIfStatement, ElseStatement? ElseStatement) : Statement;
public record IfStatementNode(Expression Condition, BodyNode Body) : IfNode;
public record ElseIfStatementNode(Expression Condition, BodyNode Body) : IfNode;
public record ElseStatement(BodyNode Body) : IfNode;