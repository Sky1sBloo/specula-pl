#include "LexerError.hpp"
#include "LexerFileReader.hpp"
#include "LexerFileWriter.hpp"
#include "LexicalAnalyzer.hpp"
#include <print>

int main(int argc, char** argv)
{
    if (argc == 0) {
        std::print("Usage: ./specula [filePath]");
    }

    LexicalAnalyzer lexer;

    const std::string filePath = argv[1];
    try {
        LexerFileReader lexerFileReader { lexer, filePath };
    } catch (const LexerError& error) {
        std::print("Lexer Error at line {}:{}\n Message: {}\n", error.getLine(), error.getCharPos(), error.what());
    }
    LexerFileWriter lexerFileWriter { lexer, filePath };
}
