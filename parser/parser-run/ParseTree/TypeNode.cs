namespace SpeculaSyntaxAnalyzer.ParseTree;

public enum DataTypes
{
    INT,
    FLOAT,
    DOUBLE,
    CHAR,
    BOOL,
    STRING,
    VOID,
    NULL,
    IDENTIFIER,
    INFER, // for var type inference
    ARRAY, // for array types
    UNKNOWN
}
public record TypeNode(DataTypes DataType, TypeNode? GenericType = null) : ParseNode;

