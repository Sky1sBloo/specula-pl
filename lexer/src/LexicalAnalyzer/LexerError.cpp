#include "LexerError.hpp"

LexerError::LexerError(const std::string& message)
    : std::runtime_error(message)
    , mLine(-1)
    , mCharPos(-1)
{
}

LexerError::LexerError(const std::string& message, int line, int charPos)
    : std::runtime_error(message)
    , mLine(line)
    , mCharPos(charPos)
{
}
