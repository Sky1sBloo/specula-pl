add_library(specula-lexer
    LexicalAnalyzer.cpp
    LexerRuleset.cpp
)

target_include_directories(specula-lexer PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
