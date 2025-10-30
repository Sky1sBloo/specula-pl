#include "LexicalAnalyzer.hpp"
#include "Tokens.hpp"

const std::unordered_map<std::string_view, TokenType> LexicalAnalyzer::mKeywords {
    { "let", TokenType::K_LET },
    { "int", TokenType::K_TYPE }
};

const std::unordered_map<std::string_view, TokenType> LexicalAnalyzer::mOperators {
    { "=", TokenType::OP_EQUALS }
};

const std::unordered_map<char, TokenType> LexicalAnalyzer::mDelimeters {
    { ' ', TokenType::SPACE },
    { ';', TokenType::SEMICOLON },
    { ':', TokenType::COLON },
    { '(', TokenType::PARENTHESIS_OPEN },
    { ')', TokenType::PARENTHESIS_CLOSE },
    { '[', TokenType::BRACKET_OPEN },
    { ']', TokenType::BRACKET_CLOSE },
    { '{', TokenType::CURLY_BRACKET_OPEN },
    { '}', TokenType::CURLY_BRACKET_CLOSE },
};
