#pragma once

enum class TokenType {
    K_LET,
    K_TYPE,

    OP_EQUALS,
    OP_PLUS,
    OP_MINUS,
    OP_MULT,
    OP_DIVIDE,
    OP_MOD,

    OP_EQ_COMP,
    OP_PLUS_EQ,
    OP_MINUS_EQ,
    OP_MULT_EQ,
    OP_DIV_EQ,
    OP_MOD_EQ,

    OP_INCREMENT,
    OP_DECREMENT,

    PARENTHESIS_OPEN,
    PARENTHESIS_CLOSE,
    BRACKET_OPEN,
    BRACKET_CLOSE,

    COLON,
    SEMICOLON,
    IDENTIFIER,
    SPACE, // Does not need to be included in the final vector
    UNKNOWN
};
