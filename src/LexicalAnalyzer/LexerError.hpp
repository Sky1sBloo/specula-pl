#pragma once

#include <stdexcept>

class LexerError : std::runtime_error {
public:
    LexerError(const std::string& message);

    LexerError(const std::string& message, int line, int charPos);

    int getLine() const { return mLine; }
    int getCharPos() const { return mCharPos; }

private:
    int mLine;
    int mCharPos;
};
