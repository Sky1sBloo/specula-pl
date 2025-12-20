#include <gtest/gtest.h>
#include <ranges>

#include "LexerError.hpp"
#include "LexicalAnalyzer.hpp"
#include "Tokens.hpp"

TEST(LEXER_TEST, KEYWORD_IDENT)
{
    const std::string varDecl = "let int;ident:let power; puff if else ifelse export_default contract";
    LexicalAnalyzer lexer { varDecl };

    const std::vector<Token>& tokens = lexer.getTokens();
    const std::array<TokenType, 14> expectedTokens = {
        TokenType::K_LET,
        TokenType::K_TYPE,
        TokenType::D_SEMICOLON,
        TokenType::IDENT,
        TokenType::D_COLON,
        TokenType::K_LET,
        TokenType::IDENT,
        TokenType::D_SEMICOLON,
        TokenType::IDENT,
        TokenType::K_IF,
        TokenType::K_ELSE,
        TokenType::IDENT,
        TokenType::K_EXPORT_DEFAULT,
        TokenType::K_CONTRACT
    };

    ASSERT_EQ(tokens.size(), expectedTokens.size());
    for (const auto& [token, expectedToken] : std::views::zip(tokens, expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }
}

TEST(LEXER_TEST, IDENT_WITH_DASH)
{
    const std::string test = "init-state auto-reset auto-move auto-potato init-reset auto--move init - state";
    LexicalAnalyzer lexer { test };
    const std::vector<Token>& tokens = lexer.getTokens();
    const std::array<TokenType, 16> expectedTokens = {
        TokenType::K_INIT_STATE,
        TokenType::K_AUTO_RESET,
        TokenType::K_AUTO_MOVE,
        TokenType::IDENT,
        TokenType::OP_MINUS,
        TokenType::IDENT,
        TokenType::IDENT,
        TokenType::OP_MINUS,
        TokenType::IDENT,
        TokenType::IDENT,
        TokenType::OP_MINUS,
        TokenType::OP_MINUS,
        TokenType::K_MOVE,
        TokenType::IDENT,
        TokenType::OP_MINUS,
        TokenType::K_STATE,
    };
    ASSERT_EQ(tokens.size(), expectedTokens.size());
    for (const auto& [token, expectedToken] : std::views::zip(tokens, expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }
}

TEST(LEXER_TEST, LS)
{
    const std::string test = "1234 123.4 1234.f 123. false; true 'a' '\n'";
    LexicalAnalyzer lexer { test };

    const std::vector<Token>& tokens = lexer.getTokens();
    const std::array<TokenType, 9> expectedTokens = {
        TokenType::L_INT,
        TokenType::L_DOUBLE,
        TokenType::L_FLOAT,
        TokenType::L_DOUBLE,
        TokenType::L_BOOL,
        TokenType::D_SEMICOLON,
        TokenType::L_BOOL,
        TokenType::L_CHAR,
        TokenType::L_CHAR
    };

    ASSERT_EQ(tokens.size(), expectedTokens.size());
    for (const auto& [token, expectedToken] : std::views::zip(tokens, expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }

    // Test flush
    const std::array<std::string, 7> testFlush = { "1234", "234.f", "234.0", "123.02", "true", "'a'", "'\n'" };
    const std::array<TokenType, 7> expectedTokensFlush = {
        TokenType::L_INT,
        TokenType::L_FLOAT,
        TokenType::L_DOUBLE,
        TokenType::L_DOUBLE,
        TokenType::L_BOOL,
        TokenType::L_CHAR,
        TokenType::L_CHAR,
    };

    for (const auto& [str, expectedTok] : std::views::zip(testFlush, expectedTokensFlush)) {
        lexer.reset();
        lexer.buildTokens(str);
        ASSERT_EQ(tokens.size(), 1);
        EXPECT_EQ(expectedTok, tokens[0].type);
    }

    // Should fail
    const std::string expectFail[] = { "'aa'", "'", "''" };
    const int expectCharPos[] = { 3, 1, 2 };
    for (const auto& [str, charPos] : std::views::zip(expectFail, expectCharPos)) {
        lexer.reset();
        try {
            lexer.buildTokens(str);
        } catch (const LexerError& ex) {
            EXPECT_EQ(ex.getCharPos(), charPos);
        } catch (...) {
            FAIL() << "Expected Lexer Error";
        }
    }
}

TEST(LEXER_TEST, L_STR)
{
    const std::string test = "\"Test\" \"\"";
    LexicalAnalyzer lexer { test };
    const std::array<TokenType, 2> expectedTokens = {
        TokenType::L_STRING,
        TokenType::L_STRING
    };

    ASSERT_EQ(lexer.getTokens().size(), expectedTokens.size());
    for (const auto& [token, expectedToken] : std::views::zip(lexer.getTokens(), expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }
    // Test flush
    const std::string testFlush[] = { "\"Test\"", "\"\"" };
    const TokenType expectedTokensFlush[] = {
        TokenType::L_STRING,
        TokenType::L_STRING
    };
    ASSERT_EQ(lexer.getTokens().size(), expectedTokens.size());
    for (const auto& [str, expectedTok] : std::views::zip(testFlush, expectedTokensFlush)) {
        lexer.reset();
        lexer.buildTokens(str);
        ASSERT_EQ(lexer.getTokens().size(), 1);
        EXPECT_EQ(expectedTok, lexer.getTokens()[0].type);
    }

    // should fail
    const std::string expectFail[] = { "\"", "\"Test" };
    for (const std::string& str : expectFail) {
        lexer.reset();
        EXPECT_THROW(lexer.buildTokens(str), LexerError);
    }
}

TEST(LEXER_TEST, ESCAPE_CHAR)
{
    const std::string testStr = "\"Potato\\\"\" \"\\n\" \"\\\\\"";
    LexicalAnalyzer lexer { testStr };
    const std::array<Token, 3> expectedTokensStr = {
        Token { TokenType::L_STRING, "Potato\"" },
        Token { TokenType::L_STRING, "\n" },
        Token { TokenType::L_STRING, "\\" }
    };

    ASSERT_EQ(lexer.getTokens().size(), expectedTokensStr.size());
    for (const auto& [token, expectedToken] : std::views::zip(lexer.getTokens(), expectedTokensStr)) {
        EXPECT_EQ(expectedToken.type, token.type);
        EXPECT_EQ(expectedToken.value, token.value);
    }

    // Should fail
    const std::string teststrfail = "\"\\";
    lexer.reset();
    EXPECT_THROW(lexer.buildTokens(teststrfail), LexerError);

    const std::string testChar = "'\\n' '\\\\' '\\\''";
    const std::array<Token, 3> expectedTokensChar = {
        Token { TokenType::L_CHAR, "\n" },
        Token { TokenType::L_CHAR, "\\" },
        Token { TokenType::L_CHAR, "\'" }
    };
    lexer.reset();
    lexer.buildTokens(testChar);

    ASSERT_EQ(lexer.getTokens().size(), expectedTokensChar.size());
    for (const auto& [token, expectedToken] : std::views::zip(lexer.getTokens(), expectedTokensChar)) {
        EXPECT_EQ(expectedToken.type, token.type);
        EXPECT_EQ(expectedToken.value, token.value);
    }
    const std::string testCharFail = "\'\\";
    lexer.reset();
    EXPECT_THROW(lexer.buildTokens(testCharFail), LexerError);
}

TEST(LEXER_TEST, OPERATORS)
{
    const std::string test = "+-/ * *=== =/ +++--.. &&& ^^ ||| !<<< >>>";
    LexicalAnalyzer lexer { test };

    const std::vector<Token>& tokens = lexer.getTokens();
    const std::array<TokenType, 24> expectedTokens = {
        TokenType::OP_PLUS,
        TokenType::OP_MINUS,
        TokenType::OP_DIVIDE,
        TokenType::OP_MULT,
        TokenType::OP_MULT_EQ,
        TokenType::OP_REL_EQ,
        TokenType::OP_EQUALS,
        TokenType::OP_DIVIDE,
        TokenType::OP_INCR,
        TokenType::OP_PLUS,
        TokenType::OP_DECR,
        TokenType::OP_PERIOD,
        TokenType::OP_PERIOD,
        TokenType::OP_AND,
        TokenType::OP_BITW_AND,
        TokenType::OP_BITW_XOR,
        TokenType::OP_BITW_XOR,
        TokenType::OP_OR,
        TokenType::OP_BITW_OR,
        TokenType::OP_NOT,
        TokenType::OP_SHIFT_L,
        TokenType::OP_REL_LESS,
        TokenType::OP_SHIFT_R,
        TokenType::OP_REL_GREATER
    };
    ASSERT_EQ(tokens.size(), expectedTokens.size());
    for (const auto& [token, expectedToken] : std::views::zip(tokens, expectedTokens)) {
        //EXPECT_EQ(tokenTypeToString.at(expectedToken), tokenTypeToString.at(token.type));
        EXPECT_EQ(expectedToken, token.type);
    }

    // Test flush
    const std::array<std::string, 19> testFlush = { "+", "-", "/", "=", "+=", "-=", "/=", "++",
        "--", "&", "&&", "||", "|", "^", "<", ">", "<<", ">>", "!" };
    const TokenType expectedTokensFlush[] = {
        TokenType::OP_PLUS,
        TokenType::OP_MINUS,
        TokenType::OP_DIVIDE,
        TokenType::OP_EQUALS,
        TokenType::OP_PLUS_EQ,
        TokenType::OP_MINUS_EQ,
        TokenType::OP_DIV_EQ,
        TokenType::OP_INCR,
        TokenType::OP_DECR,
        TokenType::OP_BITW_AND,
        TokenType::OP_AND,
        TokenType::OP_OR,
        TokenType::OP_BITW_OR,
        TokenType::OP_BITW_XOR,
        TokenType::OP_REL_LESS,
        TokenType::OP_REL_GREATER,
        TokenType::OP_SHIFT_L,
        TokenType::OP_SHIFT_R,
        TokenType::OP_NOT,
    };

    for (const auto& [str, expectedTok] : std::views::zip(testFlush, expectedTokensFlush)) {
        lexer.reset();
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

    const std::array<TokenType, 8> expectedTokens = {
        TokenType::OP_MINUS_EQ,
        TokenType::OP_DECR,
        TokenType::OP_LEFT_OP,
        TokenType::OP_MINUS,
        TokenType::OP_SHIFT_L,
        TokenType::OP_BIDIR_OP,
        TokenType::OP_RIGHT_OP,
        TokenType::OP_REL_GREATER
    };

    ASSERT_EQ(tokens.size(), expectedTokens.size());
    for (const auto& [token, expectedToken] : std::views::zip(tokens, expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }

    // Test flush
    const std::array<std::string, 8> testFlush = { "--", "-=", "<-", "<", "-", "->", "<->", ">" };
    const std::array<TokenType, 8> expectedTokensFlush = {
        TokenType::OP_DECR,
        TokenType::OP_MINUS_EQ,
        TokenType::OP_LEFT_OP,
        TokenType::OP_REL_LESS,
        TokenType::OP_MINUS,
        TokenType::OP_RIGHT_OP,
        TokenType::OP_BIDIR_OP,
        TokenType::OP_REL_GREATER
    };

    for (const auto& [str, expectedTok] : std::views::zip(testFlush, expectedTokensFlush)) {
        lexer.reset();
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
        TokenType::D_SEMICOLON,
        TokenType::D_PAR_OP,
        TokenType::D_BRAC_OP,
        TokenType::D_BRAC_CLO,
        TokenType::D_CBRAC_CLO,
        TokenType::D_CBRAC_OP,
        TokenType::D_COLON
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
        TokenType::L_INT,
        TokenType::D_SEMICOLON,
        TokenType::OP_PLUS,
        TokenType::L_FLOAT,
        TokenType::D_SEMICOLON,
        TokenType::IDENT,
        TokenType::OP_DIVIDE
    };

    ASSERT_EQ(tokens.size(), expectedTokens.size());

    for (const auto& [token, expectedToken] : std::views::zip(tokens, expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }
}

TEST(LEXER_TEST, MULTILINE_COMMENTS)
{
    const std::string test = "1; /*Test * lest**/ +2.5f; /* this should be ignored \n Fortuna*/ potato /";
    LexicalAnalyzer lexer { test };
    const std::vector<Token>& tokens = lexer.getTokens();
    const std::array<TokenType, 7> expectedTokens = {
        TokenType::L_INT,
        TokenType::D_SEMICOLON,
        TokenType::OP_PLUS,
        TokenType::L_FLOAT,
        TokenType::D_SEMICOLON,
        TokenType::IDENT,
        TokenType::OP_DIVIDE
    };

    ASSERT_EQ(tokens.size(), expectedTokens.size());

    for (const auto& [token, expectedToken] : std::views::zip(tokens, expectedTokens)) {
        EXPECT_EQ(expectedToken, token.type);
    }
}
