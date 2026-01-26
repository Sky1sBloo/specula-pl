namespace SpeculaSyntaxAnalyzer.ParseTree;

public enum AssignMethod
{
    MOVE,
    SHARE,
    REF
}
public interface Assignment : Statement, ForInit;
public record AssignmentStatementNode(string Identifier, Expression Value) : Assignment;
public record AssignmentMethodNode(string Identifier, AssignMethod Method, Movable Value) : Assignment;
public record AssignPlusEqNode(string Identifier, Expression Value) : Assignment;
public record AssignMinusEqNode(string Identifier, Expression Value) : Assignment;
public record AssignMulEqNode(string Identifier, Expression Value) : Assignment;
public record AssignDivEqNode(string Identifier, Expression Value) : Assignment;
public record AssignModEqNode(string Identifier, Expression Value) : Assignment;

