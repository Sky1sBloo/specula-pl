#pragma once

#include <cstddef>
#include <unordered_map>
#include <string>

enum class TokenType {
    LITERAL_INT,
    LITERAL_FLOAT,
    LITERAL_DOUBLE,
    LITERAL_CHAR,
    LITERAL_STRING,
    LITERAL_BOOL,

    K_LET,
    K_TYPE,

    K_IF,
    K_ELSE,

    K_FOR,
    K_WHILE,
    K_DO,
    K_IN,

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

    K_CONTRACT,
    K_LISTENER,
    K_STATE,
    K_INIT_STATE,
    K_FAIL,
    K_AUTO_RESET,
    K_AUTO_MOVE,
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

    OP_EQUALS,
    OP_PLUS,
    OP_MINUS,
    OP_MULT,
    OP_DIVIDE,
    OP_MOD,
    OP_PERIOD,

    OP_EQ_COMP,
    OP_PLUS_EQ,
    OP_MINUS_EQ,
    OP_MULT_EQ,
    OP_DIV_EQ,
    OP_MOD_EQ,

    OP_LESS,
    OP_GREATER,

    OP_LEFT_OP,
    OP_RIGHT_OP,
    OP_BIDIR_OP,

    OP_INCREMENT,
    OP_DECREMENT,

    PARENTHESIS_OPEN,
    PARENTHESIS_CLOSE,
    BRACKET_OPEN,
    BRACKET_CLOSE,
    CURLY_BRACKET_OPEN,
    CURLY_BRACKET_CLOSE,

    COLON,
    SEMICOLON,
    IDENTIFIER,

    // Does not need to be included in the final vector
    NEW_LINE,
    SPACE,
    TAB,
    COMMA,
    AT_SYMBOL,
    UNKNOWN
};


struct TokenTypeHash {
    std::size_t operator()(const TokenType& t) const noexcept {
        return static_cast<std::size_t>(t);
    }
};

extern const std::unordered_map<TokenType, std::string, TokenTypeHash> tokenTypeToString; 

