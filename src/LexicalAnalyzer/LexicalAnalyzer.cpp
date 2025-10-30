#include "LexicalAnalyzer.hpp"
#include "Tokens.hpp"
#include <optional>
#include <stdexcept>

LexicalAnalyzer::LexicalAnalyzer(std::string_view text)
    : mCurrentState(LexerState::START)
{
    buildTokens(text);
}

void LexicalAnalyzer::buildTokens(std::string_view text)
{
    for (char c : text) {
        mToRead = c;
        HandleStateResult result;
        do {
            result = handleState();
        } while (result == HandleStateResult::REPROCESS);
    }

    // flush leftover lexeme
    if (!mLexeme.empty()) {
        flushLeftoverLexeme();
    }
}

void LexicalAnalyzer::flushLeftoverLexeme()
{

    switch (mCurrentState) {
    case LexerState::IDENTIFIER:
        finalizeIdentifier();
        break;
    case LexerState::OP_INCREMENTABLE:
    case LexerState::OP_EQUALS_NEXT:
        mTokens.push_back({ getSingleOperatorToken(mLexeme[0]), mLexeme });
        resetState();
        break;
    default:
        break;
    }
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleState()
{
    switch (mCurrentState) {
    case LexerState::START:
        return handleStartState();
    case LexerState::IDENTIFIER:
        return handleIdentifierState();
    case LexerState::OP_EQUALS_NEXT:
        return handleOpEqualsNextState();
    case LexerState::OP_INCREMENTABLE: 
        return handleIncrementableState();
    }
    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleStartState()
{
    if (isValidIdentifier(mToRead)) {
        mCurrentState = LexerState::IDENTIFIER;
        return HandleStateResult::REPROCESS;
    } else if (isValidOperator(mToRead)) {
        mCurrentState = getOperatorStartState(mToRead);
        mLexeme.push_back(mToRead);
        return HandleStateResult::CONTINUE;
    } else {
        std::optional<TokenType> delimeter = getDelimeter(mToRead);
        if (delimeter.has_value()) {
            if (delimeter.value() != TokenType::SPACE) {
                mLexeme.push_back(mToRead);
                mTokens.push_back({ delimeter.value(), mLexeme });
            }
            resetState();
        }
    }

    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleIdentifierState()
{
    if (isValidIdentifier(mToRead)) {
        mLexeme.push_back(mToRead);
        return HandleStateResult::CONTINUE;
    }

    finalizeIdentifier();
    resetState();
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleOpEqualsNextState()
{
    if (mToRead == '=') {
        mLexeme.push_back(mToRead);
        TokenType newToken;
        switch (mLexeme[0]) {
        case '=':
            newToken = TokenType::OP_EQ_COMP;
            break;
        case '*':
            newToken = TokenType::OP_MULT_EQ;
            break;
        case '/':
            newToken = TokenType::OP_DIV_EQ;
            break;
        case '%':
            newToken = TokenType::OP_MOD_EQ;
            break;
        }
        mTokens.push_back({ newToken, mLexeme });
        resetState();
        return HandleStateResult::CONTINUE;
    }

    if (mLexeme.size() != 1) {
        throw std::runtime_error("Stored lexeme in operator is not 1");
    }

    mTokens.push_back({ getSingleOperatorToken(mLexeme[0]), mLexeme });
    resetState();
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleIncrementableState()
{
    TokenType onEqualsNext;
    bool isPrevAdd = false;

    if (mLexeme[0] != '+' && mLexeme[0] != '-') {

        throw std::runtime_error("OP_Incrementable state current lexeme is not valid");
    }
    isPrevAdd = mLexeme[0] == '+';

    char incrementChar = isPrevAdd ? '+' : '-';

    if (mToRead == '=') {
        mLexeme.push_back(mToRead);
        TokenType combinedToken = isPrevAdd ? TokenType::OP_PLUS_EQ : TokenType::OP_MINUS_EQ;
        mTokens.push_back({ combinedToken, mLexeme });
        resetState();
        return HandleStateResult::CONTINUE;
    } else if (mToRead == incrementChar) {
        mLexeme.push_back(mToRead);
        TokenType incrementToken = isPrevAdd ? TokenType::OP_INCREMENT : TokenType::OP_DECREMENT;
        mTokens.push_back({ incrementToken, mLexeme });
        resetState();
        return HandleStateResult::CONTINUE;
    }

    mTokens.push_back({ getSingleOperatorToken(mLexeme[0]), mLexeme });
    resetState();
    return HandleStateResult::REPROCESS;
}

void LexicalAnalyzer::resetState()
{
    mLexeme.clear();
    mCurrentState = LexerState::START;
}

