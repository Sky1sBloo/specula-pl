#include <sstream>
#include <string>

#include <emscripten/bind.h>

#include "LexerError.hpp"
#include "LexicalAnalyzer.hpp"
#include "TokenJsonSerializer.hpp"

namespace specula::wasm {
namespace {
void buildTokensFromBuffer(LexicalAnalyzer& lexer, const std::string& source)
{
    std::istringstream input(source);
    std::string line;
    int lineNumber = 0;
    while (std::getline(input, line)) {
        lexer.buildTokens(line, ++lineNumber);
    }
}
}

std::string tokenize(const std::string& source)
{
    LexicalAnalyzer lexer;
    try {
        buildTokensFromBuffer(lexer, source);
        return serializer::serializeTokensPayload(lexer.getTokens());
    } catch (const LexerError& error) {
        return serializer::serializeErrorPayload(error);
    }
}

} // namespace specula::wasm

EMSCRIPTEN_BINDINGS(specula_wasm_module)
{
    emscripten::function("tokenize", &specula::wasm::tokenize);
}
