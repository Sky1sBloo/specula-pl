#pragma once

#include "LexicalAnalyzer.hpp"
#include <string>

class LexerFileWriter {
public:
    LexerFileWriter(LexicalAnalyzer& lexer, const std::string& filePath);

private:
    const LexicalAnalyzer& mLexer;
};
