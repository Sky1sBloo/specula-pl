using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class DataTypeHandler : Handler
{
    public DataTypeHandler(ErrorsHandler errors) : base(errors) { }

    protected override ParseNode? verifyTokens()
    {
        switch (CurrentToken.Type)
        {
            case Token.Types.K_TYPE:
            case Token.Types.IDENT:
                {
                    TypeNode typeNode = new TypeNode(TokenTypeToDataType(CurrentToken));
                    incrementIndex();
                    return typeNode;
                }
            default:
                throw new SyntaxErrorException(["TYPE", "IDENTIFIER"], CurrentToken);
        }
    }

    public static DataTypes TokenTypeToDataType(Token token)
    {
        if (token.Type == Token.Types.K_TYPE)
        {
            return token.Value switch
            {
                "int" => DataTypes.INT,
                "float" => DataTypes.FLOAT,
                "double" => DataTypes.DOUBLE,
                "bool" => DataTypes.BOOL,
                "char" => DataTypes.CHAR,
                "string" => DataTypes.STRING,
                "void" => DataTypes.VOID,
                _ => DataTypes.UNKNOWN
            };
        }
        else if (token.Type == Token.Types.IDENT)
        {
            // Permit well-known type names that may arrive as identifiers
            return token.Value.ToLower() switch
            {
                "str" => DataTypes.STRING,
                "string" => DataTypes.STRING,
                _ => DataTypes.IDENTIFIER
            };
        }

        throw new ArgumentException("Token is not a valid data type");
    }

    public static DataTypes InferDataTypeFromTokenLiteral(Token token)
    {
        switch (token.Type)
        {
            case Token.Types.L_INT:
                return DataTypes.INT;
            case Token.Types.L_FLOAT:
                return DataTypes.FLOAT;
            case Token.Types.L_DOUBLE:
                return DataTypes.DOUBLE;
            case Token.Types.L_CHAR:
                return DataTypes.CHAR;
            case Token.Types.L_STRING:
                return DataTypes.STRING;
            case Token.Types.L_BOOL:
                return DataTypes.BOOL;
            case Token.Types.L_NULL:
                return DataTypes.NULL;
            case Token.Types.IDENT:
                return DataTypes.IDENTIFIER;
        }
        throw new ArgumentException("Failed to infer data type");
    }
}
