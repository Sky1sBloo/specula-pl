#pragma once

#include "LexicalAnalyzer.hpp"
#include "Tokens.hpp"
#include <nlohmann/json.hpp>
#include <string>

/**
 * Used for writing the output of the lexer to the file
 */
class LexerFileWriter {
public:
    LexerFileWriter(LexicalAnalyzer& lexer, const std::string& filePath);

private:
    const LexicalAnalyzer& mLexer;
    nlohmann::json mOutput;
};

inline void to_json(nlohmann::json& j, const ErrorLines& eL)
{
    j = {
        { "message", eL.message },
        { "line", eL.line },
        { "charPos", eL.charPos }
    };
}

inline void to_json(nlohmann::json& j, const Token& token)
{
    j = {
        { "type", tokenTypeToString.at(token.type) },
        { "value", token.value },
        { "char_start", token.charStart },
        { "char_end", token.charEnd },
        { "line", token.line }
    };
}
