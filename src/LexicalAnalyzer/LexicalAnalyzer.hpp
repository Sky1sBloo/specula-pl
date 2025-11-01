#pragma once

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
    EXPECT_DELIMETER,  // next character must be a delimeter
    IDENTIFIER,
    INTEGER,
    DECIMAL_REACHED,
    FLOAT,
    CHAR_START,
    CHAR_END,
    STRING_START,
    STRING,
    OP,
    OP_EQUALS_NEXT,
    OP_INCREMENTABLE,
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

    std::vector<Token> mTokens;
    static const std::unordered_map<std::string_view, TokenType> mOperators;
    static const std::unordered_map<char, TokenType> mDelimeters;
    static const std::unordered_map<std::string_view, TokenType> mKeywords;

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
    HandleStateResult handleStringState();

    HandleStateResult handleOpState();
    HandleStateResult handleOpEqualsNextState();
    HandleStateResult handleIncrementableState();

    // Saves contents from mLexeme to mTokens
    void saveToken(TokenType type);

    bool isValidIdentifier(char c);
    void finalizeIdentifier();

    bool isValidOperator(char c);
    LexerState getOperatorStartState(char c);
    TokenType getSingleOperatorToken(char c);

    std::optional<TokenType> getDelimeter(char c);
    std::optional<TokenType> getKeyword(std::string_view value);
};
