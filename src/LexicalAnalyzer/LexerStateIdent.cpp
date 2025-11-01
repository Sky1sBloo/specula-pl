#include "LexicalAnalyzer.hpp"
#include "Tokens.hpp"
#include <stdexcept>

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::setStateInvalid(std::string_view message)
{
    mCurrentState = LexerState::INVALID;
    mInvalidStateMsg = message;
    return HandleStateResult::REPROCESS;
}

bool LexicalAnalyzer::isValidIdentifier(char c)
{
    if (mCurrentState == LexerState::START && std::isdigit(c)) {
        return false;
    }

    return std::isalnum(c) || c == '_';
}

void LexicalAnalyzer::finalizeIdentifier()
{
    std::optional<TokenType> keyword = getKeyword(mLexeme);
    bool isBoolean = mLexeme == "true" || mLexeme == "false";

    if (keyword.has_value()) {
        saveToken(keyword.value());
    } else if (isBoolean) {
        saveToken(TokenType::LITERAL_BOOL);
    } else {
        saveToken(TokenType::IDENTIFIER);
    }
}

bool LexicalAnalyzer::isValidOperator(char c)
{
    switch (c) {
    case '=':
    case '*':
    case '/':
    case '%':
    case '+':
    case '-':
        return true;
    }
    return false;
}

LexerState LexicalAnalyzer::getOperatorStartState(char c)
{
    switch (c) {
    case '=':
    case '*':
    case '/':
    case '%':
        return LexerState::OP_EQUALS_NEXT;
    case '+':
    case '-':
        return LexerState::OP_INCREMENTABLE;
    }
    return LexerState::INVALID; // todo: add error state
}

TokenType LexicalAnalyzer::getSingleOperatorToken(char c)
{
    switch (c) {
    case '=':
        return TokenType::OP_EQUALS;
    case '+':
        return TokenType::OP_PLUS;
    case '-':
        return TokenType::OP_MINUS;
    case '*':
        return TokenType::OP_MULT;
    case '/':
        return TokenType::OP_DIVIDE;
    case '%':
        return TokenType::OP_MOD;
    }
    return TokenType::UNKNOWN;
}

char LexicalAnalyzer::charToEscapeChar(char c)
{
    switch (c) {
    case '\'':
        return '\'';
    case '"':
        return '\"';
    case '?':
        return '\?';
    case '\\':
        return '\\';
    case 'a':
        return '\a';
    case 'f':
        return '\f';
    case 'n':
        return '\n';
    case 'r':
        return '\r';
    case 't':
        return '\t';
    case 'v':
        return '\v';
    }
    throw std::invalid_argument("Character is not a valid escape char");
}

std::optional<TokenType> LexicalAnalyzer::getDelimeter(char c)
{
    if (mDelimeters.contains(c)) {
        return mDelimeters.at(c);
    }
    return std::nullopt;
}

std::optional<TokenType> LexicalAnalyzer::getKeyword(std::string_view value)
{
    if (mKeywords.contains(value)) {
        return mKeywords.at(value);
    }
    return std::nullopt;
}
