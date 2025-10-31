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
    case LexerState::INTEGER:
        mTokens.push_back({ TokenType::LITERAL_INT, mLexeme });
        break;
    case LexerState::DECIMAL_REACHED:
        mTokens.push_back({ TokenType::LITERAL_DOUBLE, mLexeme });
        break;
    case LexerState::FLOAT:
        mTokens.push_back({ TokenType::LITERAL_FLOAT, mLexeme });
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
    case LexerState::DELIMETER:
        return handleDelimeterState();
    case LexerState::EXPECT_DELIMETER:
        return handleExpectDelimeterState();
    case LexerState::IDENTIFIER:
        return handleIdentifierState();
    case LexerState::INTEGER:
        return handleIntegerState();
    case LexerState::DECIMAL_REACHED:
        return handleDecimalState();
    case LexerState::FLOAT:
        return handleFloatState();
    case LexerState::OP:
        return handleOpState();
    case LexerState::OP_EQUALS_NEXT:
        return handleOpEqualsNextState();
    case LexerState::OP_INCREMENTABLE:
        return handleIncrementableState();
    case LexerState::INVALID:
        throw std::runtime_error("Reached invalid state at char: " + std::string { mToRead });
        break;
    }

    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleStartState()
{
    if (isValidIdentifier(mToRead)) {
        mCurrentState = LexerState::IDENTIFIER;
        return HandleStateResult::REPROCESS;
    } else if (std::isdigit(mToRead)) {
        mCurrentState = LexerState::INTEGER;
        return HandleStateResult::REPROCESS;
    } else if (isValidOperator(mToRead)) {
        mCurrentState = LexerState::OP;
        return HandleStateResult::REPROCESS;
    } else if (getDelimeter(mToRead).has_value()) {
        mCurrentState = LexerState::DELIMETER;
        return HandleStateResult::REPROCESS;
    }

    mCurrentState = LexerState::INVALID;
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleDelimeterState()
{
    std::optional<TokenType> delimeter = getDelimeter(mToRead);
    if (delimeter.has_value()) {
        if (delimeter.value() != TokenType::SPACE) {
            mLexeme.push_back(mToRead);
            mTokens.push_back({ delimeter.value(), mLexeme });
        }
        resetState();
    }
    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleExpectDelimeterState()
{
    bool isDelimeter = getDelimeter(mToRead).has_value();
    mCurrentState = isDelimeter ? LexerState::DELIMETER : LexerState::INVALID;
    return HandleStateResult::REPROCESS;
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

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleIntegerState()
{
    if (std::isdigit(mToRead)) {
        mLexeme.push_back(mToRead);
        return HandleStateResult::CONTINUE;
    }

    if (mToRead == '.') {
        mLexeme.push_back(mToRead);
        mCurrentState = LexerState::DECIMAL_REACHED;
        return HandleStateResult::CONTINUE;
    }
    if (isValidOperator(mToRead)) {
        mTokens.push_back({ TokenType::LITERAL_INT, mLexeme });
        mCurrentState = LexerState::OP;
        return HandleStateResult::REPROCESS;
    }
    if (getDelimeter(mToRead).has_value()) {
        mCurrentState = LexerState::DELIMETER;
        return HandleStateResult::REPROCESS;
    }
    mCurrentState = LexerState::INVALID;
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleDecimalState()
{
    if (std::isdigit(mToRead)) {
        mLexeme.push_back(mToRead);
        return HandleStateResult::CONTINUE;
    }
    if (mToRead == 'f') {
        mCurrentState = LexerState::FLOAT;
        return HandleStateResult::REPROCESS;
    }
    if (isValidOperator(mToRead)) {
        mTokens.push_back({ TokenType::LITERAL_DOUBLE, mLexeme });
        mCurrentState = LexerState::OP;
        return HandleStateResult::REPROCESS;
    }

    if (getDelimeter(mToRead).has_value()) {
        mCurrentState = LexerState::DELIMETER;
        return HandleStateResult::REPROCESS;
    }
    mCurrentState = LexerState::INVALID;
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleFloatState()
{
    if (mToRead != 'f') {
        mCurrentState = LexerState::INVALID;
        return HandleStateResult::REPROCESS;
    }
    mLexeme.push_back(mToRead);
    mCurrentState = LexerState::EXPECT_DELIMETER;
    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleOpState()
{
    mCurrentState = getOperatorStartState(mToRead);
    mLexeme.push_back(mToRead);
    return HandleStateResult::CONTINUE;
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
