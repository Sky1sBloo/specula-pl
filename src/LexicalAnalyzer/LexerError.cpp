#include "LexerError.hpp"

LexerError::LexerError(const std::string& message)
    : std::runtime_error(message)
{
}

