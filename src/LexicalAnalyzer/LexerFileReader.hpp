#pragma once

#include "LexicalAnalyzer.hpp"
#include <string>

class LexerFileReader {
public:
    LexerFileReader(LexicalAnalyzer& lexer, const std::string& filePath);

private:
    const LexicalAnalyzer& mLexer;

    int mCurrentLine;

    void readFile();
};
