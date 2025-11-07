#include "LexerFileWriter.hpp"
#include <fstream>

LexerFileWriter::LexerFileWriter(LexicalAnalyzer& lexer, const std::string& filePath)
    : mLexer(lexer)
{
    std::ofstream writeFile { filePath };
    for (const Token& token : lexer.getTokens()) {
        writeFile << tokenTypeToString.at(token.type) << " | " << token.value << '\n';
    }
    writeFile.close();
}
