#pragma once

#include <stdexcept>

class LexerError : std::runtime_error {
public:
    LexerError(const std::string& message);

    LexerError(const std::string& message, char toRead, const std::string& lexeme);
};
