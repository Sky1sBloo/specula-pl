#pragma once

#include "LexicalAnalyzer.hpp"
#include <string>

/**
 * Used for writing the output of the lexer to the file
 */
class LexerFileWriter {
public:
    LexerFileWriter(LexicalAnalyzer& lexer, const std::string& filePath);

private:
    const LexicalAnalyzer& mLexer;
};
