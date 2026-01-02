cmake_minimum_required(VERSION 3.15...4.2.1)

project (specula LANGUAGES CXX)
set(CMAKE_EXPORT_COMPILE_COMMANDS ON)

include(FetchContent)
FetchContent_Declare(
  googletest
  URL https://github.com/google/googletest/archive/03597a01ee50ed33e9dfd640b249b4be3799d395.zip
)

set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
FetchContent_MakeAvailable(googletest)
add_subdirectory(src/LexicalAnalyzer)
add_subdirectory(src/json)
add_executable(specula src/main.cpp)
target_compile_features(specula PUBLIC cxx_std_23)
target_link_libraries(specula PRIVATE specula-lexer)
set_target_properties(specula PROPERTIES CXX_EXTENSIONS OFF)

add_subdirectory(tests)
