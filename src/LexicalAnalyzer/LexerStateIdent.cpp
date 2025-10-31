#include "LexicalAnalyzer.hpp"

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

    if (keyword.has_value()) {
        mTokens.push_back({ keyword.value(), mLexeme });
    } else {
        mTokens.push_back({ TokenType::IDENTIFIER, mLexeme });
    }
    mLexeme.clear();
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
