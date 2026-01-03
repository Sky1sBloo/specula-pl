#pragma once

#include <nlohmann/json.hpp>
#include <string>

struct ErrorLines {
    std::string message;
    int charPos;
    int line;
};
