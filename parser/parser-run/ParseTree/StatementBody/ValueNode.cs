namespace SpeculaSyntaxAnalyzer.ParseTree;

public interface ValueNode : Expression;
public interface Movable : ValueNode;  // be able to use move, ref, share assignments

public record LiteralValue(TypeNode Type, string Value) : ValueNode;
public record IdentifierValue(string Value) : ValueNode, Movable;
public record FunctionCallValue(string Identifier, PrintableList<Expression> Parameters) : ValueNode, Movable;

public record StructKey(string Key, Expression Value);
public record StructInitialization(string Identifier, PrintableList<StructKey> Keys) : ValueNode;
public record MemberAccessValue(Expression Object, string Member) : ValueNode, Movable;
public record MemberFunctionCallValue(Expression Object, string Method, PrintableList<Expression> Parameters) : ValueNode, Movable;
