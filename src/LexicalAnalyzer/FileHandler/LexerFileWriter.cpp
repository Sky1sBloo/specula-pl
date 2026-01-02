#include "LexerFileWriter.hpp"
#include <filesystem>
#include <fstream>

LexerFileWriter::LexerFileWriter(LexicalAnalyzer& lexer, const std::string& filePath)
    : mLexer(lexer)
{
    std::filesystem::path inputPath { filePath };
    std::filesystem::path outputPath = inputPath.parent_path() / (inputPath.stem().string() + "_tokens" + inputPath.extension().string());
    std::ofstream writeFile { outputPath };
    for (const Token& token : lexer.getTokens()) {
        writeFile << tokenTypeToString.at(token.type) << " | " << token.value << '\n';
    }
    writeFile.close();
}
