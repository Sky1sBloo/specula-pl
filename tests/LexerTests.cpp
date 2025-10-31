#include <gtest/gtest.h>
#include <ranges>

#include "LexicalAnalyzer.hpp"
#include "Tokens.hpp"

TEST(LEXER_TEST, KEYWORD_IDENTIFIER)
{
    const std::string varDecl = "let int;ident:let power; puff if else ifelse export_default contract";
    LexicalAnalyzer lexer { varDecl };

    const std::vector<Token>& tokens = lexer.getTokens();
    const std::vector<TokenType> expectedTokens = {
        TokenType::K_LET,
        TokenType::K_TYPE,
        TokenType::SEMICOLON,
        TokenType::IDENTIFIER,
        TokenType::COLON,
        TokenType::K_LET,
        TokenType::IDENTIFIER,
        TokenType::SEMICOLON,
        TokenType::IDENTIFIER,
        TokenType::K_IF,
        TokenType::K_ELSE,
        TokenType::IDENTIFIER,
        TokenType::K_EXPORT_DEFAULT,
        TokenType::K_CONTRACT
    };

    for (const auto& [token, expectedToken] : std::views::zip(tokens, expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }
}

TEST(LEXER_TEST, OPERATORS)
{
    const std::string test = "+-/ * *=== =/ +++--";
    LexicalAnalyzer lexer { test };

    const std::vector<Token>& tokens = lexer.getTokens();
    const std::vector<TokenType> expectedTokens = {
        TokenType::OP_PLUS,
        TokenType::OP_MINUS,
        TokenType::OP_DIVIDE,
        TokenType::OP_MULT,
        TokenType::OP_MULT_EQ,
        TokenType::OP_EQ_COMP,
        TokenType::OP_EQUALS,
        TokenType::OP_DIVIDE,
        TokenType::OP_INCREMENT,
        TokenType::OP_PLUS,
        TokenType::OP_DECREMENT
    };
    for (const auto& [token, expectedToken] : std::views::zip(tokens, expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }
}

TEST(LEXER_TEST, DELIMETERS)
{
    const std::string test = "let;([]}{";
    LexicalAnalyzer lexer { test };
    const std::vector<Token>& tokens = lexer.getTokens();

    const std::vector<TokenType> expectedTokens = {
        TokenType::K_LET,
        TokenType::SEMICOLON,
        TokenType::PARENTHESIS_OPEN,
        TokenType::BRACKET_OPEN,
        TokenType::BRACKET_CLOSE,
        TokenType::CURLY_BRACKET_CLOSE,
        TokenType::CURLY_BRACKET_OPEN,
    };

    for (const auto& [token, expectedToken] : std::views::zip(tokens, expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }
}
