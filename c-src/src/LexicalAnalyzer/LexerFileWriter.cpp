#include "LexerFileWriter.hpp"
#include <filesystem>
#include <fstream>

#include "TokenJsonSerializer.hpp"

LexerFileWriter::LexerFileWriter(LexicalAnalyzer& lexer, const std::string& filePath)
    : mLexer(lexer)
{
    std::filesystem::path inputPath { filePath };
    std::filesystem::path outputPath = inputPath.parent_path() / (inputPath.stem().string() + "_tokens" + inputPath.extension().string());
    std::ofstream writeFile { outputPath };
    if (!writeFile.is_open()) {
        return;
    }
    writeFile << specula::serializer::serializeTokensPayload(lexer.getTokens());
    writeFile << std::endl;
}
