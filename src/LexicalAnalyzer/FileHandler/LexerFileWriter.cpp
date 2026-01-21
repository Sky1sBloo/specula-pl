#include "LexerFileWriter.hpp"
#include <filesystem>
#include <fstream>

LexerFileWriter::LexerFileWriter(LexicalAnalyzer& lexer, const std::string& filePath)
    : mLexer(lexer)
{
    std::filesystem::path inputPath { filePath };
    std::filesystem::path outputPath = inputPath.parent_path() / (inputPath.stem().string() + "_tokens" + inputPath.extension().string());
    std::ofstream writeFile { outputPath };
    mOutput["file"] = {
        {"name", inputPath.stem().string()},
        {"type", "specula_src"}
    };
    mOutput["errors"] = lexer.getErrors();
    mOutput["tokens"] = lexer.getTokens();
    writeFile << std::setw(4) << mOutput;
   
    writeFile.close();
}
