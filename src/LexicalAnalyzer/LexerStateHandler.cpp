#include "LexerError.hpp"
#include "LexicalAnalyzer.hpp"
#include "Tokens.hpp"

void LexicalAnalyzer::flushLeftoverLexeme()
{
    switch (mCurrentState) {
    case LexerState::IDENTIFIER:
        if (!mLexeme.empty())
            finalizeIdentifier();
        break;
    case LexerState::IDENTIFIER_DASH: {
        if (!mLexeme.empty())
            finalizeIdentifierDash();
        break;
    }
    case LexerState::NUM_START:
        if (!mLexeme.empty())
            saveToken(TokenType::L_INT);
        saveToken(TokenType::UNKNOWN);
        break;
    case LexerState::DECIMAL_REACHED:
        if (!mLexeme.empty()) {
            if (mLexeme.ends_with('.')) {
                setStateInvalid("Double is not ended");
            }
            saveToken(TokenType::L_DOUBLE);
        }
        break;
    case LexerState::FLOAT:
        if (!mLexeme.empty())
            saveToken(TokenType::L_FLOAT);
        saveToken(TokenType::UNKNOWN);
        break;
    case LexerState::CHAR_START:
        setStateInvalid("Char is not ended");
        break;
    case LexerState::CHAR_END:
        if (!mLexeme.empty())
            setStateInvalid("Char is not ended");
        saveToken(TokenType::UNKNOWN);
        break;
    case LexerState::CHAR_ESCAPE_CHAR:
        if (!mLexeme.empty()) {
            handleCharEscapeCharState();
        } else {
            setStateInvalid("Character escape not ended");
            saveToken(TokenType::UNKNOWN);
        }
        break;
    case LexerState::STRING_START:
        setStateInvalid("String not ended");
        break;
    case LexerState::STRING:
        if (mToRead != '"') {
            setStateInvalid("String not ended");
            saveToken(TokenType::UNKNOWN);
        }
        handleStringState();
        break;
    case LexerState::STRING_ESCAPE_CHAR:
        if (!mLexeme.empty()) {
            handleStringEscapeCharState();
        } else {
            setStateInvalid("Character escape not ended");
            saveToken(TokenType::UNKNOWN);
        }
        break;
    case LexerState::OP_INCREMENTABLE:
    case LexerState::OP_EQUALS_NEXT:
    case LexerState::OP_MINUS:
    case LexerState::OP_LESS_THAN:
    case LexerState::OP_GREATER_THAN:
    case LexerState::OP_LOGICAL:
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
    case LexerState::MULTILINE_COMMENT:
    case LexerState::MULTILINE_COMMENT_END:
        return;
    case LexerState::INVALID:
        saveToken(TokenType::UNKNOWN);
    default:
        break;
    }

    resetState();
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
    case LexerState::IDENTIFIER_DASH:
        return handleIdentifierDashState();
    case LexerState::NUM_START:
        return handleNumStartState();
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
    case LexerState::OP_LOGICAL:
        return handleOpLogicalState();
    case LexerState::OP_MINUS:
        return handleOpMinusState();
    case LexerState::OP_LESS_THAN:
        return handleOpLessThanState();
    case LexerState::OP_GREATER_THAN:
        return handleOpGreaterThanState();
    case LexerState::OP_LEFT_ARROW:
        return handleOpLeftArrowState();
    case LexerState::CHAR_SLASH:
        return handleCharSlashState();
    case LexerState::COMMENT:
        return handleCommentState();
    case LexerState::MULTILINE_COMMENT:
        return handleMultilineCommentState();
    case LexerState::MULTILINE_COMMENT_END:
        return handleMultilineCommentEndState();
    case LexerState::INVALID:
        return handleInvalidState();
        // throw LexerError(mInvalidStateMsg, mLine, mCharPos);
    }

    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleStartState()
{
    if (isValidIdentifier(mToRead)) {
        mCurrentState = LexerState::IDENTIFIER;
        return HandleStateResult::REPROCESS;
    } else if (std::isdigit(mToRead)) {
        mCurrentState = LexerState::NUM_START;
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

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleInvalidState()
{
    bool isDelimeter = getDelimeter(mToRead).has_value();
    if (!isDelimeter) {
        mLexeme.push_back(mToRead);
        return HandleStateResult::CONTINUE;
    }
    saveToken(TokenType::UNKNOWN);
    mCurrentState = LexerState::DELIMETER;
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleDelimeterState()
{
    std::optional<TokenType> delimeter = getDelimeter(mToRead);
    if (delimeter.has_value()) {
        bool isIgnore = false;
        switch (delimeter.value()) {
        case TokenType::SPACE:
        case TokenType::NEW_LINE:
        case TokenType::TAB:
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
    } else {
        return setStateInvalid("Cannot identify delimeter");
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

    // Check if its a keyword with a dash
    const std::string keywordsWithDash[] = { "init", "auto" };
    if (mToRead == '-') {
        for (const std::string& kWithDash : keywordsWithDash) {
            if (mLexeme == kWithDash) {
                mLexeme.push_back(mToRead);
                mCurrentState = LexerState::IDENTIFIER_DASH;
                return HandleStateResult::CONTINUE;
            }
        }
    }

    finalizeIdentifier();
    resetState();
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleIdentifierDashState()
{
    if (isValidIdentifier(mToRead)) {
        mLexeme.push_back(mToRead);
        return HandleStateResult::CONTINUE;
    }

    finalizeIdentifierDash();
    resetState();
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleNumStartState()
{
    if (std::isdigit(static_cast<unsigned char>(mToRead))) {
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
    saveToken(TokenType::L_INT);
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
    LexerState nextState = LexerState::INVALID;
    if (isValidOperator(mToRead)) {
        nextState = LexerState::OP;
    } else if (getDelimeter(mToRead).has_value()) {
        nextState = LexerState::DELIMETER;
    } else {
        return setStateInvalid("Decimal state does not recognize character: " + std::string { mToRead });
    }
    if (nextState == LexerState::INVALID) {
        throw LexerError("Decimal state not ended");
    }

    if (mLexeme.ends_with('.')) {
        return setStateInvalid("Decimal ends with .");
    }

    saveToken(TokenType::L_DOUBLE);
    mCurrentState = nextState;
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleFloatState()
{
    if (mToRead != 'f') {
        return setStateInvalid("Float state postfix is not f");
    }
    mLexeme.push_back(mToRead);
    saveToken(TokenType::L_FLOAT);
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
    }

    for (char newLine : mForceStringEscape) {
        if (newLine == mToRead) {
            setStateInvalid("New line before string close");
        }
    }

    mLexeme.push_back(mToRead);
    mCurrentState = LexerState::CHAR_END;

    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleCharEndState()
{
    if (mToRead != '\'') {
        return setStateInvalid("Character length is more than 1");
    }
    saveToken(TokenType::L_CHAR);
    mCurrentState = LexerState::START;
    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleCharEscapeCharState()
{
    for (char c : escapeChar) {
        if (mToRead == c) {
            try {
                mLexeme.push_back(charToEscapeChar(mToRead));
            } catch (const std::invalid_argument& ex) {
                return setStateInvalid(ex.what());
            }
            mCurrentState = LexerState::CHAR_END;
            return HandleStateResult::CONTINUE;
        }
    }

    return setStateInvalid("Character escape state does not recognize character: " + std::string { mToRead });
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleStringState()
{
    if (mToRead == '"') {
        saveToken(TokenType::L_STRING);
        mCurrentState = LexerState::START;
        return HandleStateResult::CONTINUE;
    }
    if (mToRead == '\\') {
        mCurrentState = LexerState::STRING_ESCAPE_CHAR;
        return HandleStateResult::CONTINUE;
    }
    // check if it contains newline
    for (char newLine : mForceStringEscape) {
        if (newLine == mToRead) {
            setStateInvalid("New line before string close");
        }
    }
    mLexeme.push_back(mToRead);
    mCurrentState = LexerState::STRING;
    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleStringEscapeCharState()
{
    for (char c : escapeChar) {
        if (mToRead == c) {
            try {
                mLexeme.push_back(charToEscapeChar(mToRead));
            } catch (const std::invalid_argument& ex) {
                return setStateInvalid(ex.what());
            }

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
    case '>': {
        mCurrentState = LexerState::OP_GREATER_THAN;
        mLexeme.push_back(mToRead);
        return HandleStateResult::CONTINUE;
    }
    case '&':
    case '|': {
        mCurrentState = LexerState::OP_LOGICAL;
        mLexeme.push_back(mToRead);
        return HandleStateResult::CONTINUE;
    }
    case '^':
        mLexeme.push_back(mToRead);
        saveToken(TokenType::OP_BITW_XOR);
        return HandleStateResult::CONTINUE;
    case '!':
        mLexeme.push_back(mToRead);
        saveToken(TokenType::OP_NOT);
        return HandleStateResult::CONTINUE;
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
            newToken = TokenType::OP_REL_EQ;
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
        return setStateInvalid("Stored lexeme in operator is not 1");
    }

    saveToken(getSingleOperatorToken(mLexeme[0]));
    resetState();
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleIncrementableState()
{
    TokenType onEqualsNext;
    bool isPrevAdd = false;

    /*
    if (mLexeme[0] != '+' && mLexeme[0] != '-') {
    } */

    if (mLexeme.size() != 1) {
        return setStateInvalid("mLexeme is not 1 on entering incrementable " + std::string { mLexeme });
    }
    isPrevAdd = mLexeme[0] == '+';

    char incrementChar = isPrevAdd ? '+' : '-';

    if (mToRead == '=') {
        mCurrentState = LexerState::OP_EQUALS_NEXT;
        return HandleStateResult::REPROCESS;
    } else if (mToRead == incrementChar) {
        mLexeme.push_back(mToRead);
        TokenType incrementToken = isPrevAdd ? TokenType::OP_INCR : TokenType::OP_DECR;
        saveToken(incrementToken);
        resetState();
        return HandleStateResult::CONTINUE;
    }

    saveToken(getSingleOperatorToken(mLexeme[0]));
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleOpLogicalState()
{
    if (mToRead == '=') {
        mCurrentState = LexerState::OP_EQUALS_NEXT;
        return HandleStateResult::REPROCESS;
    }
    if (mLexeme.size() != 1) {
        return setStateInvalid("mLexeme is not 1 on entering op logical state");
    }

    char firstChar = mLexeme.at(0);

    if (mToRead == firstChar) {
        mLexeme.push_back(mToRead);
        switch (mToRead) {
        case '&':
            saveToken(TokenType::OP_AND);
            break;
        case '|':
            saveToken(TokenType::OP_OR);
            break;
        default:
            return setStateInvalid("mToRead is an invalid character in logical state");
        }
        return HandleStateResult::CONTINUE;
    }

    saveToken(getSingleOperatorToken(firstChar));
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
        mLexeme.push_back(mToRead);
        saveToken(TokenType::OP_REL_LESS_EQ);
        return HandleStateResult::CONTINUE;
    }
    case '-': {
        mCurrentState = LexerState::OP_LEFT_ARROW;
        mLexeme.push_back(mToRead);
        return HandleStateResult::CONTINUE;
    case '<': {
        mLexeme.push_back(mToRead);
        saveToken(TokenType::OP_SHIFT_L);
        return HandleStateResult::CONTINUE;
    }
    }
    }
    saveToken(TokenType::OP_REL_LESS);
    return HandleStateResult::REPROCESS;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleOpGreaterThanState()
{
    switch (mToRead) {
    case '=': {
        mLexeme.push_back(mToRead);
        saveToken(TokenType::OP_REL_GREATER_EQ);
        return HandleStateResult::CONTINUE;
    }
    case '>': {
        mLexeme.push_back(mToRead);
        saveToken(TokenType::OP_SHIFT_R);
        return HandleStateResult::CONTINUE;
    }
    }
    saveToken(TokenType::OP_REL_GREATER);
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
    } else if (mToRead == '*') {
        mLexeme.clear();
        mCurrentState = LexerState::MULTILINE_COMMENT;
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

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleMultilineCommentState()
{
    if (mToRead == '*') {
        mCurrentState = LexerState::MULTILINE_COMMENT_END;
    }
    return HandleStateResult::CONTINUE;
}

LexicalAnalyzer::HandleStateResult LexicalAnalyzer::handleMultilineCommentEndState()
{
    if (mToRead == '/') {
        mCurrentState = LexerState::START;
    } else if (mToRead == '*') {
        return HandleStateResult::CONTINUE;
    } else {
        mCurrentState = LexerState::MULTILINE_COMMENT;
    }
    return HandleStateResult::CONTINUE;
}
