#include "LexerFileReader.hpp"
#include <fstream>

LexerFileReader::LexerFileReader(LexicalAnalyzer& lexer, const std::string& filePath)
    : mLexer(lexer)
    , mCurrentLine(0)
{
    std::ifstream readFile { filePath };
    std::string line;

    while (std::getline(readFile, line)) {
        mCurrentLine++;
        lexer.buildTokens(line);
    }
}
