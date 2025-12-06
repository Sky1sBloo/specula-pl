#include "LexerFileReader.hpp"
#include <fstream>
#include <stdexcept>

LexerFileReader::LexerFileReader(LexicalAnalyzer& lexer, const std::string& filePath)
    : mLexer(lexer)
    , mCurrentLine(0)
{
    std::ifstream readFile { filePath };
    std::string line;
    if (!readFile.is_open()) {
        throw std::invalid_argument("Cannot open file: " + filePath);
    }

    while (std::getline(readFile, line)) {
        lexer.buildTokens(line, ++mCurrentLine);
    }
}
