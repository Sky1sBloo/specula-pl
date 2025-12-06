add_library(specula-lexer
    LexicalAnalyzer.cpp
    LexerStateHandler.cpp
    LexerRuleset.cpp
    LexerHelperFunc.cpp
    LexerError.cpp
    LexerFileReader.cpp
    LexerFileWriter.cpp
    TokenJsonSerializer.cpp
    Tokens.cpp
)

target_include_directories(specula-lexer PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
