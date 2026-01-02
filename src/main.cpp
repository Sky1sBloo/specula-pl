#include "LexerError.hpp"
#include "FileHandler/LexerFileReader.hpp"
#include "FileHandler/LexerFileWriter.hpp"
#include "LexicalAnalyzer.hpp"
#include <print>
#include <stdexcept>

int main(int argc, char** argv)
{
    if (argc < 2) {
        std::print("Usage: ./specula [filePath] ...");
    }
    std::vector<std::string> files;
    files.reserve(argc);
    for (int i = 1; i < argc; i++) {
        files.push_back(argv[i]);
    }

    LexicalAnalyzer lexer;

    for (const std::string& file : files) {
        try {
            LexerFileReader lexerFileReader { lexer, file };
        } catch (const LexerError& error) {
            std::print("Lexer Error at line {}:{}\n Message: {}\n", error.getLine(), error.getCharPos(), error.what());
        } catch (const std::invalid_argument& iErr) {
            std::print("{}\n", iErr.what());
        }
        if (!lexer.getTokens().empty()) {
            LexerFileWriter lexerFileWriter { lexer, file };
            lexer.reset();
        }
    }
}
