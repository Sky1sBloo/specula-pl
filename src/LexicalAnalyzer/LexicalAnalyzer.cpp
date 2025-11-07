#include "LexicalAnalyzer.hpp"
#include "Tokens.hpp"

LexicalAnalyzer::LexicalAnalyzer(std::string_view text)
    : mCurrentState(LexerState::START)
    , mInvalidStateMsg("")
    , mLine(0)
    , mCharPos(0)
{
    buildTokens(text);
}

LexicalAnalyzer::LexicalAnalyzer()
    : mCurrentState(LexerState::START)
    , mInvalidStateMsg("")
    , mLine(0)
    , mCharPos(0)
{
}

void LexicalAnalyzer::reset()
{
    resetState();
    mTokens.clear();
    mLine = 0;
    mCharPos = 0;
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

void LexicalAnalyzer::buildTokens(std::string_view text, int line)
{
    mCharPos = 0;
    for (char c : text) {
        mToRead = c;
        HandleStateResult result;
        do {
            result = handleState();
        } while (result == HandleStateResult::REPROCESS);

        mCharPos++;
    }

    flushLeftoverLexeme();
}
