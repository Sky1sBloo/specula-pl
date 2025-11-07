#include "LexerFileReader.hpp"
#include "LexerError.hpp"
#include <fstream>
#include <print>

LexerFileReader::LexerFileReader(LexicalAnalyzer& lexer, const std::string& filePath)
    : mLexer(lexer)
    , mCurrentLine(0)
{
    std::ifstream readFile { filePath };
    std::string line;

    while (std::getline(readFile, line)) {
        mCurrentLine++;
        try {
            lexer.buildTokens(line);
        } catch (const LexerError& error) {
            std::print("Lexer Error at line {}:{}\n Message: {}", error.getLine(), error.getCharPos(), error.what());
        }
    }
}
