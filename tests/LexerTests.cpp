#include <gtest/gtest.h>
#include <ranges>

#include "LexerError.hpp"
#include "LexicalAnalyzer.hpp"
#include "Tokens.hpp"

TEST(LEXER_TEST, KEYWORD_IDENTIFIER)
{
    const std::string varDecl = "let int;ident:let power; puff if else ifelse export_default contract";
    LexicalAnalyzer lexer { varDecl };

    const std::vector<Token>& tokens = lexer.getTokens();
    const std::array<TokenType, 14> expectedTokens = {
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

TEST(LEXER_TEST, LITERALS)
{
    const std::string test = "1234 123.4 1234.f 123. false; true 'a' '\n'";
    LexicalAnalyzer lexer { test };

    const std::vector<Token>& tokens = lexer.getTokens();
    const std::array<TokenType, 9> expectedTokens = {
        TokenType::LITERAL_INT,
        TokenType::LITERAL_DOUBLE,
        TokenType::LITERAL_FLOAT,
        TokenType::LITERAL_DOUBLE,
        TokenType::LITERAL_BOOL,
        TokenType::SEMICOLON,
        TokenType::LITERAL_BOOL,
        TokenType::LITERAL_CHAR,
        TokenType::LITERAL_CHAR
    };

    ASSERT_EQ(tokens.size(), expectedTokens.size());
    for (const auto& [token, expectedToken] : std::views::zip(tokens, expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }

    // Test flush
    const std::array<std::string, 7> testFlush = { "1234", "234.f", "234.", "123.02", "true", "'a'", "'\n'" };
    const std::array<TokenType, 7> expectedTokensFlush = {
        TokenType::LITERAL_INT,
        TokenType::LITERAL_FLOAT,
        TokenType::LITERAL_DOUBLE,
        TokenType::LITERAL_DOUBLE,
        TokenType::LITERAL_BOOL,
        TokenType::LITERAL_CHAR,
        TokenType::LITERAL_CHAR,
    };

    for (const auto& [str, expectedTok] : std::views::zip(testFlush, expectedTokensFlush)) {
        lexer.buildTokens(str);
        ASSERT_EQ(tokens.size(), 1);
        EXPECT_EQ(expectedTok, tokens[0].type);
    }

    // Should fail
    const std::string expectFail[] = { "'aa'", "'", "''" };
    for (const std::string& str : expectFail) {
        EXPECT_THROW(lexer.buildTokens(str), LexerError);
    }
}

TEST(LEXER_TEST, LITERAL_STR)
{
    const std::string test = "\"Test\" \"\"";
    LexicalAnalyzer lexer { test };
    const std::array<TokenType, 2> expectedTokens = {
        TokenType::LITERAL_STRING,
        TokenType::LITERAL_STRING
    };

    ASSERT_EQ(lexer.getTokens().size(), expectedTokens.size());
    for (const auto& [token, expectedToken] : std::views::zip(lexer.getTokens(), expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }
    // Test flush
    const std::string testFlush[] = { "\"Test\"", "\"\"" };
    const TokenType expectedTokensFlush[] = {
        TokenType::LITERAL_STRING,
        TokenType::LITERAL_STRING
    };
    ASSERT_EQ(lexer.getTokens().size(), expectedTokens.size());
    for (const auto& [str, expectedTok] : std::views::zip(testFlush, expectedTokensFlush)) {
        lexer.buildTokens(str);
        ASSERT_EQ(lexer.getTokens().size(), 1);
        EXPECT_EQ(expectedTok, lexer.getTokens()[0].type);
    }

    // should fail
    const std::string expectFail[] = { "\"", "\"Test" };
    for (const std::string& str : expectFail) {
        EXPECT_THROW(lexer.buildTokens(str), LexerError);
    }
}

TEST(LEXER_TEST, ESCAPE_CHAR)
{
    const std::string testStr = "\"Potato\\\"\" \"\\n\" \"\\\\\"";
    LexicalAnalyzer lexer { testStr };
    const std::array<Token, 3> expectedTokensStr = {
        Token { TokenType::LITERAL_STRING, "Potato\"" },
        Token { TokenType::LITERAL_STRING, "\n" },
        Token { TokenType::LITERAL_STRING, "\\" }
    };

    ASSERT_EQ(lexer.getTokens().size(), expectedTokensStr.size());
    for (const auto& [token, expectedToken] : std::views::zip(lexer.getTokens(), expectedTokensStr)) {
        EXPECT_EQ(expectedToken.type, token.type);
        EXPECT_EQ(expectedToken.value, token.value);
    }

    // Should fail
    const std::string teststrfail = "\"\\";
    EXPECT_THROW(lexer.buildTokens(teststrfail), LexerError);

    const std::string testChar = "'\\n' '\\\\' '\\\''";
    const std::array<Token, 3> expectedTokensChar = {
        Token { TokenType::LITERAL_CHAR, "\n" },
        Token { TokenType::LITERAL_CHAR, "\\" },
        Token { TokenType::LITERAL_CHAR, "\'" }
    };
    lexer.buildTokens(testChar);

    ASSERT_EQ(lexer.getTokens().size(), expectedTokensChar.size());
    for (const auto& [token, expectedToken] : std::views::zip(lexer.getTokens(), expectedTokensChar)) {
        EXPECT_EQ(expectedToken.type, token.type);
        EXPECT_EQ(expectedToken.value, token.value);
    }
    const std::string testCharFail = "\'\\";
    EXPECT_THROW(lexer.buildTokens(testCharFail), LexerError);
}

TEST(LEXER_TEST, OPERATORS)
{
    const std::string test = "+-/ * *=== =/ +++--..";
    LexicalAnalyzer lexer { test };

    const std::vector<Token>& tokens = lexer.getTokens();
    const std::array<TokenType, 13> expectedTokens = {
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
        TokenType::OP_DECREMENT,
        TokenType::OP_PERIOD,
        TokenType::OP_PERIOD
    };
    ASSERT_EQ(tokens.size(), expectedTokens.size());
    for (const auto& [token, expectedToken] : std::views::zip(tokens, expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }

    // Test flush
    const std::array<std::string, 9> testFlush = { "+", "-", "/", "=", "+=", "-=", "/=", "++", "--" };
    const TokenType expectedTokensFlush[] = {
        TokenType::OP_PLUS,
        TokenType::OP_MINUS,
        TokenType::OP_DIVIDE,
        TokenType::OP_EQUALS,
        TokenType::OP_PLUS_EQ,
        TokenType::OP_MINUS_EQ,
        TokenType::OP_DIV_EQ,
        TokenType::OP_INCREMENT,
        TokenType::OP_DECREMENT
    };

    for (const auto& [str, expectedTok] : std::views::zip(testFlush, expectedTokensFlush)) {
        lexer.buildTokens(str);
        ASSERT_EQ(lexer.getTokens().size(), 1);
        EXPECT_EQ(expectedTok, lexer.getTokens()[0].type);
    }
}

TEST(LEXER_TEST, OP_ARROWS)
{
    const std::string test = "-=--<--<<<->->>";
    LexicalAnalyzer lexer { test };

    const std::vector<Token>& tokens = lexer.getTokens();

    const std::array<TokenType, 9> expectedTokens = {
        TokenType::OP_MINUS_EQ,
        TokenType::OP_DECREMENT,
        TokenType::OP_LEFT_OP,
        TokenType::OP_MINUS,
        TokenType::OP_LESS,
        TokenType::OP_LESS,
        TokenType::OP_BIDIR_OP,
        TokenType::OP_RIGHT_OP,
        TokenType::OP_GREATER
    };

    ASSERT_EQ(tokens.size(), expectedTokens.size());
    for (const auto& [token, expectedToken] : std::views::zip(tokens, expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }

    // Test flush
    const std::array<std::string, 8> testFlush = { "--", "-=", "<-", "<" , "-", "->", "<->", ">" };
    const std::array<TokenType, 8> expectedTokensFlush = {
        TokenType::OP_DECREMENT,
        TokenType::OP_MINUS_EQ,
        TokenType::OP_LEFT_OP,
        TokenType::OP_LESS,
        TokenType::OP_MINUS,
        TokenType::OP_RIGHT_OP,
        TokenType::OP_BIDIR_OP,
        TokenType::OP_GREATER
    };

    for (const auto& [str, expectedTok] : std::views::zip(testFlush, expectedTokensFlush)) {
        lexer.buildTokens(str);
        ASSERT_EQ(lexer.getTokens().size(), 1);
        EXPECT_EQ(expectedTok, lexer.getTokens()[0].type);
    }
}

TEST(LEXER_TEST, DELIMETERS)
{
    const std::string test = "let;([]}{:";
    LexicalAnalyzer lexer { test };
    const std::vector<Token>& tokens = lexer.getTokens();

    const std::array<TokenType, 8> expectedTokens = {
        TokenType::K_LET,
        TokenType::SEMICOLON,
        TokenType::PARENTHESIS_OPEN,
        TokenType::BRACKET_OPEN,
        TokenType::BRACKET_CLOSE,
        TokenType::CURLY_BRACKET_CLOSE,
        TokenType::CURLY_BRACKET_OPEN,
        TokenType::COLON
    };

    ASSERT_EQ(tokens.size(), expectedTokens.size());
    for (const auto& [token, expectedToken] : std::views::zip(tokens, expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }
}

TEST(LEXER_TEST, COMMENTS)
{
    const std::string test = "1; +2.5f; // this should be ignored \n potato /";
    LexicalAnalyzer lexer { test };
    const std::vector<Token>& tokens = lexer.getTokens();

    const std::array<TokenType, 7> expectedTokens = {
        TokenType::LITERAL_INT,
        TokenType::SEMICOLON,
        TokenType::OP_PLUS,
        TokenType::LITERAL_FLOAT,
        TokenType::SEMICOLON,
        TokenType::IDENTIFIER,
        TokenType::OP_DIVIDE
    };

    ASSERT_EQ(tokens.size(), expectedTokens.size());

    for (const auto& [token, expectedToken] : std::views::zip(tokens, expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }
}
