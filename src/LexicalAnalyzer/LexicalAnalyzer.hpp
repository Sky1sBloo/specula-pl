#pragma once

#include <array>
#include <optional>
#include <string>
#include <string_view>
#include <unordered_map>
#include <vector>

#include "Tokens.hpp"

struct Token {
    TokenType type;
    std::string value;
};

enum class LexerState {
    START,
    DELIMETER,
    EXPECT_DELIMETER, // next character must be a delimeter
    IDENTIFIER,
    INTEGER,
    DECIMAL_REACHED,
    FLOAT,
    CHAR_START,
    CHAR_END,
    CHAR_ESCAPE_CHAR,
    STRING_START,
    STRING,
    STRING_ESCAPE_CHAR,
    OP,
    OP_EQUALS_NEXT,
    OP_INCREMENTABLE,
    CHAR_SLASH,  
    OP_MINUS,
    OP_LESS_THAN,
    OP_LEFT_ARROW,
    COMMENT,
    INVALID
};

class LexicalAnalyzer {
public:
    LexicalAnalyzer(std::string_view text);

    void buildTokens(std::string_view text);
    const std::vector<Token>& getTokens() const { return mTokens; }

private:
    LexerState mCurrentState;
    char mToRead;
    std::string mLexeme; // to be appended by build tokens
    std::string mInvalidStateMsg;

    std::vector<Token> mTokens;
    static const std::unordered_map<std::string_view, TokenType> mOperators;
    static const std::unordered_map<char, TokenType> mDelimeters;
    static const std::unordered_map<std::string_view, TokenType> mKeywords;
    static constexpr std::array<char, 11> escapeChar = { '\'', '"', '\\', '?', 'a', 'b', 'f', 'n', 'r', 't', 'v' };

    enum class HandleStateResult {
        CONTINUE,
        REPROCESS // When handleState doesn't store the character to the lexeme (mainly for exiting states)
    };
    HandleStateResult handleState();
    void resetState();
    void flushLeftoverLexeme();

    HandleStateResult handleStartState();
    HandleStateResult handleDelimeterState();
    HandleStateResult handleExpectDelimeterState();
    HandleStateResult handleIdentifierState();

    HandleStateResult handleIntegerState();
    HandleStateResult handleDecimalState();
    HandleStateResult handleFloatState();
    HandleStateResult handleCharStartState();
    HandleStateResult handleCharEndState();
    HandleStateResult handleCharEscapeCharState();
    HandleStateResult handleStringState();
    HandleStateResult handleStringEscapeCharState();

    HandleStateResult handleOpState();
    HandleStateResult handleOpEqualsNextState();
    HandleStateResult handleIncrementableState();
    HandleStateResult handleOpMinusState();
    HandleStateResult handleOpLessThanState();
    HandleStateResult handleOpLeftArrowState();

    HandleStateResult handleCharSlashState();
    HandleStateResult handleCommentState();

    // Saves contents from mLexeme to mTokens
    void saveToken(TokenType type);

    HandleStateResult setStateInvalid(std::string_view message);

    bool isValidIdentifier(char c);
    void finalizeIdentifier();

    bool isValidOperator(char c);
    LexerState getOperatorStartState(char c);
    TokenType getSingleOperatorToken(char c);

    // Appends backslash before it
    char charToEscapeChar(char c);

    std::optional<TokenType> getDelimeter(char c);
    std::optional<TokenType> getKeyword(std::string_view value);
};
