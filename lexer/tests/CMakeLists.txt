add_executable(
    specula-unit-tests
    LexerTests.cpp
)

target_link_libraries(
    specula-unit-tests
    PRIVATE
    specula-lexer
    GTest::gtest_main
)

include(GoogleTest)
gtest_discover_tests(specula-unit-tests)
