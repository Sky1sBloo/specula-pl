#include "LexicalAnalyzer.hpp"
#include "Tokens.hpp"
#include <optional>
#include <stdexcept>

LexicalAnalyzer::LexicalAnalyzer(std::string_view text)
    : mCurrentState(LexerState::START)
{
    buildTokens(text);
}

void LexicalAnalyzer::resetState()
{
    mLexeme.clear();
    mCurrentState = LexerState::START;
}

void LexicalAnalyzer::saveToken(TokenType type)
{
    mTokens.push_back({ type, mLexeme });
    mLexeme.clear();
}

void LexicalAnalyzer::buildTokens(std::string_view text)
{
    resetState();
    mTokens.clear();
    for (char c : text) {
        mToRead = c;
        HandleStateResult result;
        do {
            result = handleState();
        } while (result == HandleStateResult::REPROCESS);
    }

    flushLeftoverLexeme();
}

void LexicalAnalyzer::flushLeftoverLexeme()
{

    switch (mCurrentState) {
    case LexerState::IDENTIFIER:
        if (!mLexeme.empty())
            finalizeIdentifier();
        break;
    case LexerState::INTEGER:
        if (!mLexeme.empty())
            saveToken(TokenType::LITERAL_INT);
        break;
    case LexerState::DECIMAL_REACHED:
        if (!mLexeme.empty())
            saveToken(TokenType::LITERAL_DOUBLE);
        break;
    case LexerState::FLOAT:
        if (!mLexeme.empty())
            saveToken(TokenType::LITERAL_FLOAT);
        break;
    case LexerState::CHAR_START:
        throw std::runtime_error("Char start is not ended");
        break;
    case LexerState::CHAR_END:
        if (!mLexeme.empty())
            handleCharEndState();
        break;
    case LexerState::OP_INCREMENTABLE:
    case LexerState::OP_EQUALS_NEXT:
        if (!mLexeme.empty())
            saveToken(getSingleOperatorToken(mLexeme[0]));
        break;
    default:
        break;
    }
    if (mCurrentState == LexerState::INVALID) {
        throw std::runtime_error("Lexer went to invalid state at: mToRead: " + std::string { mToRead } + " at lexeme: " + mLexeme);
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
    case LexerState::CHAR_START:
        return handleCharStartState();
    case LexerState::CHAR_END:
        return handleCharEndState();
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
    } else if (mToRead == '\'') {
        mCurrentState = LexerState::CHAR_START;
        return HandleStateResult::CONTINUE;
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
            saveToken(delimeter.value());
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
        mCurrentState = LexerState::OP;
    } else if (getDelimeter(mToRead).has_value()) {
        mCurrentState = LexerState::DELIMETER;
    } else {
        mCurrentState = LexerState::INVALID;
    }
    mTokens.push_back({ TokenType::LITERAL_INT, mLexeme });
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
        mCurrentState = LexerState::OP;
    } else if (getDelimeter(mToRead).has_value()) {
        mCurrentState = LexerState::DELIMETER;
    } else {
        mCurrentState = LexerState::INVALID;
    }
    saveToken(TokenType::LITERAL_DOUBLE);
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleFloatState()
{
    if (mToRead != 'f') {
        mCurrentState = LexerState::INVALID;
        return HandleStateResult::REPROCESS;
    }
    mLexeme.push_back(mToRead);
    saveToken(TokenType::LITERAL_FLOAT);
    mCurrentState = LexerState::EXPECT_DELIMETER;
    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleCharStartState()
{
    if (mToRead != '\'') {
        mLexeme.push_back(mToRead);
        saveToken(TokenType::LITERAL_CHAR);
    }
    mCurrentState = LexerState::CHAR_END;
    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleCharEndState()
{
    if (mToRead != '\'') {
        mCurrentState = LexerState::INVALID;
        return HandleStateResult::REPROCESS;
    }
    saveToken(TokenType::LITERAL_CHAR);
    mCurrentState = LexerState::START;
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
        saveToken(newToken);
        resetState();
        return HandleStateResult::CONTINUE;
    }

    if (mLexeme.size() != 1) {
        throw std::runtime_error("Stored lexeme in operator is not 1");
    }

    saveToken(getSingleOperatorToken(mLexeme[0]));
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
