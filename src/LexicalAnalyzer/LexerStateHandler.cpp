#include "LexerError.hpp"
#include "LexicalAnalyzer.hpp"
#include <optional>
#include "Tokens.hpp"

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
        throw LexerError("Char start is not ended");
        break;
    case LexerState::CHAR_END:
        if (!mLexeme.empty())
            handleCharEndState();
        break;
    case LexerState::CHAR_ESCAPE_CHAR:
        if (!mLexeme.empty()) {
            handleCharEscapeCharState();
        } else {
            throw LexerError("Character escape not ended");
        }
        break;
    case LexerState::STRING_START:
        throw LexerError("String not ended");
        break;
    case LexerState::STRING:
        if (mToRead != '"') {
            throw LexerError("String not ended");
        }
        handleStringState();
        break;
    case LexerState::STRING_ESCAPE_CHAR:
        if (!mLexeme.empty()) {
            handleStringEscapeCharState();
        } else {
            throw LexerError("Character escape not ended");
        }
        break;
    case LexerState::OP_INCREMENTABLE:
    case LexerState::OP_EQUALS_NEXT:
    case LexerState::OP_MINUS:
    case LexerState::OP_LESS_THAN:
        if (!mLexeme.empty())
            saveToken(getSingleOperatorToken(mLexeme[0]));
        break;
    case LexerState::CHAR_SLASH:
        if (!mLexeme.empty())
            saveToken(getSingleOperatorToken(mLexeme[0]));
        break;
    case LexerState::OP_LEFT_ARROW:
        if (!mLexeme.empty())
            saveToken(TokenType::OP_LEFT_OP);
    default:
        break;
    }
    if (mCurrentState == LexerState::INVALID) {
        throw LexerError("INVALID_STATE " + mInvalidStateMsg + "\nReached invalid state at char '" + std::string { mToRead } + "' at lexeme: \"" + mLexeme + "\"  ");
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
    case LexerState::CHAR_ESCAPE_CHAR:
        return handleCharEscapeCharState();
    case LexerState::STRING_START:
        return handleStringState();
    case LexerState::STRING:
        return handleStringState();
    case LexerState::STRING_ESCAPE_CHAR:
        return handleStringEscapeCharState();
    case LexerState::OP:
        return handleOpState();
    case LexerState::OP_EQUALS_NEXT:
        return handleOpEqualsNextState();
    case LexerState::OP_INCREMENTABLE:
        return handleIncrementableState();
    case LexerState::OP_MINUS:
        return handleOpMinusState();
    case LexerState::OP_LESS_THAN:
        return handleOpLessThanState();
    case LexerState::OP_LEFT_ARROW:
        return handleOpLeftArrowState();
    case LexerState::CHAR_SLASH:
        return handleCharSlashState();
    case LexerState::COMMENT:
        return handleCommentState();
    case LexerState::INVALID:
        throw LexerError("INVALID_STATE " + mInvalidStateMsg + "\nReached invalid state at char '" + std::string { mToRead } + "' at lexeme: \"" + mLexeme + "\"  ");
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
    } else if (mToRead == '"') {
        mCurrentState = LexerState::STRING_START;
        return HandleStateResult::CONTINUE;
    } else if (getDelimeter(mToRead).has_value()) {
        mCurrentState = LexerState::DELIMETER;
        return HandleStateResult::REPROCESS;
    }

    return setStateInvalid("Unrecognized initial character");
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleDelimeterState()
{
    std::optional<TokenType> delimeter = getDelimeter(mToRead);
    if (delimeter.has_value()) {
        bool isIgnore = false;
        switch (delimeter.value()) {
        case TokenType::SPACE:
        case TokenType::NEW_LINE:
            isIgnore = true;
            break;
        default:
            isIgnore = false;
        }
        if (!isIgnore) {
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
    if (!isDelimeter) {
        return setStateInvalid("Expect delimeter state doesn't receive delimeter");
    }
    mCurrentState = LexerState::DELIMETER;
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
        return setStateInvalid("Integer state does not recognize character: " + std::string { mToRead });
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
        return setStateInvalid("Decimal state does not recognize character: " + std::string { mToRead });
    }
    saveToken(TokenType::LITERAL_DOUBLE);
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleFloatState()
{
    if (mToRead != 'f') {
        return setStateInvalid("Float state postfix is not f");
    }
    mLexeme.push_back(mToRead);
    saveToken(TokenType::LITERAL_FLOAT);
    mCurrentState = LexerState::EXPECT_DELIMETER;
    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleCharStartState()
{
    if (mToRead == '\\') {
        mCurrentState = LexerState::CHAR_ESCAPE_CHAR;
        return HandleStateResult::CONTINUE;
    }

    if (mToRead == '\'') {
        return setStateInvalid("Character is empty");
    } else {
        mLexeme.push_back(mToRead);
        mCurrentState = LexerState::CHAR_END;
    }

    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleCharEndState()
{
    if (mToRead != '\'') {
        return setStateInvalid("Character length is more than 1");
    }
    saveToken(TokenType::LITERAL_CHAR);
    mCurrentState = LexerState::START;
    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleCharEscapeCharState()
{
    for (char c : escapeChar) {
        if (mToRead == c) {
            mLexeme.push_back(charToEscapeChar(mToRead));
            mCurrentState = LexerState::CHAR_END;
            return HandleStateResult::CONTINUE;
        }
    }

    return setStateInvalid("Character escape state does not recognize character: " + std::string { mToRead });
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleStringState()
{
    if (mToRead == '"') {
        saveToken(TokenType::LITERAL_STRING);
        mCurrentState = LexerState::START;
        return HandleStateResult::CONTINUE;
    }
    if (mToRead == '\\') {
        mCurrentState = LexerState::STRING_ESCAPE_CHAR;
        return HandleStateResult::CONTINUE;
    }
    mLexeme.push_back(mToRead);
    mCurrentState = LexerState::STRING;
    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleStringEscapeCharState()
{
    for (char c : escapeChar) {
        if (mToRead == c) {
            mLexeme.push_back(charToEscapeChar(mToRead));
            mCurrentState = LexerState::STRING;
            return HandleStateResult::CONTINUE;
        }
    }

    return setStateInvalid("String escape state does not recognize character: " + std::string { mToRead });
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleOpState()
{
    switch (mToRead) {
    case '/': {
        mCurrentState = LexerState::CHAR_SLASH;
        mLexeme.push_back(mToRead);
        return HandleStateResult::CONTINUE;
    }
    case '-': {
        mCurrentState = LexerState::OP_MINUS;
        mLexeme.push_back(mToRead);
        return HandleStateResult::CONTINUE;
    }
    case '<': {
        mCurrentState = LexerState::OP_LESS_THAN;
        mLexeme.push_back(mToRead);
        return HandleStateResult::CONTINUE;
    }
    }

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
        case '+':
            newToken = TokenType::OP_PLUS_EQ;
            break;
        case '-':
            newToken = TokenType::OP_MINUS_EQ;
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
        throw LexerError("Stored lexeme in operator is not 1");
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
        return setStateInvalid("OP_Incrementable state current lexeme is not valid");
    }

    isPrevAdd = mLexeme[0] == '+';

    char incrementChar = isPrevAdd ? '+' : '-';

    if (mToRead == '=') {
        mCurrentState = LexerState::OP_EQUALS_NEXT;
        return HandleStateResult::REPROCESS;
    } else if (mToRead == incrementChar) {
        mLexeme.push_back(mToRead);
        TokenType incrementToken = isPrevAdd ? TokenType::OP_INCREMENT : TokenType::OP_DECREMENT;
        saveToken(incrementToken);
        resetState();
        return HandleStateResult::CONTINUE;
    }

    mTokens.push_back({ getSingleOperatorToken(mLexeme[0]), mLexeme });
    resetState();
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleOpMinusState()
{
    switch (mToRead) {
    case '-': {
        mCurrentState = LexerState::OP_INCREMENTABLE;
        return HandleStateResult::REPROCESS;
    }
    case '=': {
        mCurrentState = LexerState::OP_EQUALS_NEXT;
        return HandleStateResult::REPROCESS;
    }
    case '>': {
        mLexeme.push_back(mToRead);
        saveToken(TokenType::OP_RIGHT_OP);
        resetState();
        return HandleStateResult::CONTINUE;
    }
    }
    saveToken(TokenType::OP_MINUS);
    resetState();
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleOpLessThanState()
{
    switch (mToRead) {
    case '=': {
        mCurrentState = LexerState::OP_EQUALS_NEXT;
        return HandleStateResult::REPROCESS;
    }
    case '-': {
        mCurrentState = LexerState::OP_LEFT_ARROW;
        mLexeme.push_back(mToRead);
        return HandleStateResult::CONTINUE;
    }
    }
    saveToken(TokenType::OP_LESS);
    resetState();
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleOpLeftArrowState()
{
    if (mToRead == '>') {
        mLexeme.push_back(mToRead);
        saveToken(TokenType::OP_BIDIR_OP);
        resetState();
        return HandleStateResult::CONTINUE;
    }
    saveToken(TokenType::OP_LEFT_OP);
    resetState();
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleCharSlashState()
{
    if (mToRead == '/') {
        mLexeme.clear();
        mCurrentState = LexerState::COMMENT;
        return HandleStateResult::CONTINUE;
    } else {
        mCurrentState = LexerState::OP_EQUALS_NEXT;
        return HandleStateResult::REPROCESS;
    }
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleCommentState()
{
    if (mToRead == '\n') {
        mCurrentState = LexerState::START;
    }
    return HandleStateResult::CONTINUE;
}
