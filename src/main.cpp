#include "LexerError.hpp"
#include "LexerFileReader.hpp"
#include "LexicalAnalyzer.hpp"
#include <print>

int main(int argc, char** argv)
{
    if (argc == 0) {
        std::print("Usage: ./specula [filePath]");
    }

    LexicalAnalyzer lexer;

    try {
        LexerFileReader lexerFileReader { lexer, argv[1] };
    } catch (const LexerError& error) {
        std::print("Lexer Error at line {}:{}\n Message: {}\n", error.getLine(), error.getCharPos(), error.what());
    }
}
