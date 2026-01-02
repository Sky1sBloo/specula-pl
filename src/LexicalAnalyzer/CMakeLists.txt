FetchContent_Declare(json URL https://github.com/nlohmann/json/releases/download/v3.12.0/json.tar.xz)
FetchContent_MakeAvailable(json)
add_library(specula-lexer
    LexicalAnalyzer.cpp
    LexerStateHandler.cpp
    LexerRuleset.cpp
    LexerHelperFunc.cpp
    LexerError.cpp
    FileHandler/LexerFileReader.cpp
    FileHandler/LexerFileWriter.cpp
    Tokens.cpp
)

target_include_directories(specula-lexer PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
target_link_libraries(specula-lexer PRIVATE nlohmann_json::nlohmann_json)
target_compile_features(specula-lexer PUBLIC cxx_std_23)
set_target_properties(specula-lexer PROPERTIES CXX_EXTENSIONS OFF)
