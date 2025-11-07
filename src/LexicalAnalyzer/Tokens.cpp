#include "Tokens.hpp"

const std::unordered_map<TokenType, std::string, TokenTypeHash> tokenTypeToString = {
    { TokenType::LITERAL_INT, "LITERAL_INT" },
    { TokenType::LITERAL_FLOAT, "LITERAL_FLOAT" },
    { TokenType::LITERAL_DOUBLE, "LITERAL_DOUBLE" },
    { TokenType::LITERAL_CHAR, "LITERAL_CHAR" },
    { TokenType::LITERAL_STRING, "LITERAL_STRING" },
    { TokenType::LITERAL_BOOL, "LITERAL_BOOL" },

    { TokenType::K_LET, "K_LET" },
    { TokenType::K_TYPE, "K_TYPE" },

    { TokenType::K_IF, "K_IF" },
    { TokenType::K_ELSE, "K_ELSE" },

    { TokenType::K_FOR, "K_FOR" },
    { TokenType::K_WHILE, "K_WHILE" },
    { TokenType::K_DO, "K_DO" },
    { TokenType::K_IN, "K_IN" },

    { TokenType::K_RET, "K_RET" },
    { TokenType::K_FN, "K_FN" },

    { TokenType::K_STRUCT, "K_STRUCT" },
    { TokenType::K_INTERFACE, "K_INTERFACE" },
    { TokenType::K_IMPL, "K_IMPL" },
    { TokenType::K_SELF, "K_SELF" },
    { TokenType::K_THIS, "K_THIS" },

    { TokenType::K_IMPORT, "K_IMPORT" },
    { TokenType::K_EXPORT, "K_EXPORT" },
    { TokenType::K_EXPORT_DEFAULT, "K_EXPORT_DEFAULT" },

    { TokenType::K_CONTRACT, "K_CONTRACT" },
    { TokenType::K_LISTENER, "K_LISTENER" },
    { TokenType::K_STATE, "K_STATE" },
    { TokenType::K_FAIL, "K_FAIL" },
    { TokenType::K_AUTO_RESET, "K_AUTO_RESET" },
    { TokenType::K_AUTO_MOVE, "K_AUTO_MOVE" },
    { TokenType::K_ROLES, "K_ROLES" },
    { TokenType::K_RESPOND, "K_RESPOND" },
    { TokenType::K_ON, "K_ON" },
    { TokenType::K_LISTEN, "K_LISTEN" },
    { TokenType::K_TARGET, "K_TARGET" },
    { TokenType::K_AS, "K_AS" },
    { TokenType::K_USING, "K_USING" },
    { TokenType::K_AFTER, "K_AFTER" },
    { TokenType::K_BEFORE, "K_BEFORE" },

    { TokenType::K_ASYNC, "K_ASYNC" },
    { TokenType::K_AWAIT, "K_AWAIT" },
    { TokenType::K_THREAD, "K_THREAD" },
    { TokenType::K_SPAWN, "K_SPAWN" },

    { TokenType::OP_EQUALS, "OP_EQUALS" },
    { TokenType::OP_PLUS, "OP_PLUS" },
    { TokenType::OP_MINUS, "OP_MINUS" },
    { TokenType::OP_MULT, "OP_MULT" },
    { TokenType::OP_DIVIDE, "OP_DIVIDE" },
    { TokenType::OP_MOD, "OP_MOD" },
    { TokenType::OP_PERIOD, "OP_PERIOD" },

    { TokenType::OP_EQ_COMP, "OP_EQ_COMP" },
    { TokenType::OP_PLUS_EQ, "OP_PLUS_EQ" },
    { TokenType::OP_MINUS_EQ, "OP_MINUS_EQ" },
    { TokenType::OP_MULT_EQ, "OP_MULT_EQ" },
    { TokenType::OP_DIV_EQ, "OP_DIV_EQ" },
    { TokenType::OP_MOD_EQ, "OP_MOD_EQ" },

    { TokenType::OP_LESS, "OP_LESS" },
    { TokenType::OP_GREATER, "OP_GREATER" },

    { TokenType::OP_LEFT_OP, "OP_LEFT_OP" },
    { TokenType::OP_RIGHT_OP, "OP_RIGHT_OP" },
    { TokenType::OP_BIDIR_OP, "OP_BIDIR_OP" },

    { TokenType::OP_INCREMENT, "OP_INCREMENT" },
    { TokenType::OP_DECREMENT, "OP_DECREMENT" },

    { TokenType::PARENTHESIS_OPEN, "PARENTHESIS_OPEN" },
    { TokenType::PARENTHESIS_CLOSE, "PARENTHESIS_CLOSE" },
    { TokenType::BRACKET_OPEN, "BRACKET_OPEN" },
    { TokenType::BRACKET_CLOSE, "BRACKET_CLOSE" },
    { TokenType::CURLY_BRACKET_OPEN, "CURLY_BRACKET_OPEN" },
    { TokenType::CURLY_BRACKET_CLOSE, "CURLY_BRACKET_CLOSE" },

    { TokenType::COLON, "COLON" },
    { TokenType::SEMICOLON, "SEMICOLON" },
    { TokenType::IDENTIFIER, "IDENTIFIER" },

    { TokenType::NEW_LINE, "NEW_LINE" },
    { TokenType::SPACE, "SPACE" },
    { TokenType::UNKNOWN, "UNKNOWN" },
    { TokenType::TAB, "TAB" },
    { TokenType::COMMA, "COMMA" },
    { TokenType::AT_SYMBOL, "AT_SYMBOL" }
};
