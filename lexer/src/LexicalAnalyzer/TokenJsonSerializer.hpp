#pragma once

#include <string>
#include <vector>

class LexerError;
struct Token;

namespace specula::serializer {

std::string serializeTokensPayload(const std::vector<Token>& tokens);
std::string serializeErrorPayload(const LexerError& error);

} // namespace specula::serializer
