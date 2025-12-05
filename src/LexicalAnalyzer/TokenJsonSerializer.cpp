#include "TokenJsonSerializer.hpp"

#include <array>
#include <string_view>

#include "LexerError.hpp"
#include "LexicalAnalyzer.hpp"
#include "Tokens.hpp"

namespace specula::serializer {
namespace {
std::string escapeJson(std::string_view value)
{
    std::string escaped;
    escaped.reserve(value.size() + (value.size() / 4));
    for (char c : value) {
        switch (c) {
        case '"':
            escaped += "\\\"";
            break;
        case '\\':
            escaped += "\\\\";
            break;
        case '\b':
            escaped += "\\b";
            break;
        case '\f':
            escaped += "\\f";
            break;
        case '\n':
            escaped += "\\n";
            break;
        case '\r':
            escaped += "\\r";
            break;
        case '\t':
            escaped += "\\t";
            break;
        default: {
            unsigned char uc = static_cast<unsigned char>(c);
            if (uc < 0x20) {
                constexpr std::array<char, 16> hex = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
                escaped += "\\u00";
                escaped += hex[(uc >> 4) & 0x0F];
                escaped += hex[uc & 0x0F];
            } else {
                escaped += c;
            }
            break;
        }
        }
    }
    return escaped;
}

const std::string& tokenTypeToName(const TokenType type)
{
    static const std::string unknownToken = "UNKNOWN";
    const auto it = tokenTypeToString.find(type);
    if (it != tokenTypeToString.end()) {
        return it->second;
    }
    return unknownToken;
}
}

std::string serializeTokensPayload(const std::vector<Token>& tokens)
{
    std::string json;
    json.reserve(tokens.size() * 48 + 32);
    json += "{\"ok\":true,\"tokens\":[";
    for (std::size_t index = 0; index < tokens.size(); ++index) {
        const Token& token = tokens[index];
        json += "{\"type\":\"";
        json += tokenTypeToName(token.type);
        json += "\",\"value\":\"";
        json += escapeJson(token.value);
        json += "\"}";
        if (index + 1 < tokens.size()) {
            json += ',';
        }
    }
    json += "]}";
    return json;
}

std::string serializeErrorPayload(const LexerError& error)
{
    std::string json;
    json.reserve(96);
    json += "{\"ok\":false,\"error\":{\"message\":\"";
    json += escapeJson(error.what());
    json += "\",\"line\":";
    json += std::to_string(error.getLine());
    json += ",\"char\":";
    json += std::to_string(error.getCharPos());
    json += "}}";
    return json;
}

} // namespace specula::serializer
