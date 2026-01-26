using System.Text.Json.Serialization;
namespace SpeculaSyntaxAnalyzer;

public class Token
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Types Type { get; set; }
    public string Value { get; set; } = "";
    public int Line { get; set; }
    [JsonPropertyName("char_start")]
    public int CharStart { get; set; }
    [JsonPropertyName("char_end")]
    public int CharEnd { get; set; }

    public enum Types
    {
        L_INT,
        L_FLOAT,
        L_DOUBLE,
        L_CHAR,
        L_STRING,
        L_BOOL,
        L_URL,
        L_PORT,
        L_NULL,

        K_LET,
        K_TYPE,

        K_IF,
        K_ELSE,

        K_FOR,
        K_WHILE,
        K_DO,
        K_IN,
        K_BREAK,

        K_RET,
        K_FN,

        K_STRUCT,
        K_INTERFACE,
        K_IMPL,
        K_SELF,
        K_THIS,

        K_IMPORT,
        K_EXPORT,
        K_EXPORT_DEFAULT,
        K_FROM,

        K_CONTRACT,
        K_LISTENER,
        K_STATE,
        K_INIT_STATE,
        K_FAIL,
        K_AUTO_RESET,
        K_AUTO_MOVE,
        K_TO,
        K_ROLES,
        K_RESPOND,
        K_ON,
        K_LISTEN,
        K_TARGET,
        K_AS,
        K_USING,
        K_AFTER,
        K_BEFORE,

        K_ASYNC,
        K_AWAIT,
        K_THREAD,
        K_SPAWN,

        K_OWN,
        K_MOVE,
        K_SHARED,
        K_VIEW,
        K_SHARE,
        K_REF,
        K_MUT,
        K_CONST,
        K_THR_LOCAL,
        K_SYNC,
        K_INFER,
        K_NETWORK,

        OP_EQUALS,
        OP_PLUS,
        OP_MINUS,
        OP_MULT,
        OP_DIVIDE,
        OP_MOD,
        OP_PERIOD,

        OP_REL_EQ,
        OP_REL_NOT_EQ,
        OP_REL_LESS_EQ,
        OP_REL_GREATER_EQ,
        OP_REL_LESS,
        OP_REL_GREATER,

        OP_PLUS_EQ,
        OP_MINUS_EQ,
        OP_MULT_EQ,
        OP_DIV_EQ,
        OP_MOD_EQ,


        OP_AND,
        OP_OR,
        OP_NOT,

        OP_BITW_AND,
        OP_BITW_OR,
        OP_BITW_XOR,

        OP_SHIFT_L,
        OP_SHIFT_R,

        OP_LEFT_OP,
        OP_RIGHT_OP,
        OP_BIDIR_OP,

        OP_INCR,
        OP_DECR,

        D_PAR_OP,
        D_PAR_CLO,
        D_BRAC_OP,
        D_BRAC_CLO,
        D_CBRAC_OP,
        D_CBRAC_CLO,
        D_COLON,
        D_SEMICOLON,
        IDENT,

        NEW_LINE,
        SPACE,
        TAB,
        COMMA,
        AT_SYMBOL,
        UNKNOWN
    };
}
