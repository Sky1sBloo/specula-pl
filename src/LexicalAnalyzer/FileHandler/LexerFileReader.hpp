#pragma once

#include "LexicalAnalyzer.hpp"
#include <string>

/**
 * Mainly used for reading the file for the lexer
 * Automatically builds tokens on construct
 */
class LexerFileReader {
public:
    LexerFileReader(LexicalAnalyzer& lexer, const std::string& filePath);

private:
    const LexicalAnalyzer& mLexer;

    int mCurrentLine;
};
