#include "ErrorLines.hpp"
#include "LexicalAnalyzer.hpp"
#include "Tokens.hpp"
#include <stdexcept>
#include <string>

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::setStateInvalid(std::string message)
{
    mCurrentState = LexerState::INVALID;
    mErrors.push_back(ErrorLines { message, mCharPos, mLine });
    return HandleStateResult::REPROCESS;
}

bool LexicalAnalyzer::isValidIdentifier(char c)
{
    if (mCurrentState == LexerState::START && std::isdigit(static_cast<unsigned char>(c))) {
        return false;
    }

    return std::isalnum(static_cast<unsigned char>(c)) || c == '_';
}

void LexicalAnalyzer::finalizeIdentifier()
{
    std::optional<TokenType> keyword = getKeyword(mLexeme);
    bool isBoolean = mLexeme == "true" || mLexeme == "false";

    if (keyword.has_value()) {
        saveToken(keyword.value());
    } else if (isBoolean) {
        saveToken(TokenType::L_BOOL);
    } else {
        saveToken(TokenType::IDENT);
    }
}

void LexicalAnalyzer::finalizeIdentifierDash()
{
    std::optional<TokenType> keyword = getKeyword(mLexeme);

    if (keyword.has_value()) {
        saveToken(keyword.value());
    } else {
        std::size_t pos = mLexeme.find('-');
        if (pos == std::string::npos) {
            setStateInvalid("On identifier dash end state but no dash is found");
        }
        std::string leftIdent = mLexeme.substr(0, pos);
        std::string rightIdent = mLexeme.substr(pos + 1);
        if (!leftIdent.empty()) {
            mTokens.push_back({ TokenType::IDENT, leftIdent });
        }
        mTokens.push_back({ TokenType::OP_MINUS, "-" });
        if (!rightIdent.empty()) {
            mTokens.push_back({ TokenType::IDENT, rightIdent });
        }
        mLexeme.clear();
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
    case '.':
    case '<':
    case '>':
    case '&':
    case '|':
    case '!':
    case '^':
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
    case '.':
    case '<':
    case '>':
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
    case '.':
        return TokenType::OP_PERIOD;
    case '<':
        return TokenType::OP_REL_LESS;
    case '>':
        return TokenType::OP_REL_GREATER;
    case '&':
        return TokenType::OP_BITW_AND;
    case '|':
        return TokenType::OP_BITW_OR;
    case '^':
        return TokenType::OP_BITW_XOR;
    case '!':
        return TokenType::OP_NOT;
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
