cmake_minimum_required(VERSION 3.30)

project (specula LANGUAGES CXX)
set(CMAKE_CXX_STANDARD 23)
set(CMAKE_CXX_STANDARD_REQUIRED YES)
set(CMAKE_EXPORT_COMPILE_COMMANDS ON)

option(SPECULA_BUILD_WASM "Build the WebAssembly tokenizer module" OFF)
if (SPECULA_BUILD_WASM AND NOT CMAKE_SYSTEM_NAME MATCHES "Emscripten")
  message(FATAL_ERROR "SPECULA_BUILD_WASM requires configuring the project with the Emscripten toolchain (emcmake cmake ...)")
endif()

include(FetchContent)
FetchContent_Declare(
  googletest
  URL https://github.com/google/googletest/archive/03597a01ee50ed33e9dfd640b249b4be3799d395.zip
)

set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
FetchContent_MakeAvailable(googletest)
add_subdirectory(src/LexicalAnalyzer)
add_executable(specula src/main.cpp)
target_link_libraries(specula PRIVATE specula-lexer)

if (SPECULA_BUILD_WASM)
  add_executable(specula-wasm src/wasm/SpeculaWasm.cpp)
  target_link_libraries(specula-wasm PRIVATE specula-lexer)
  target_compile_options(specula-wasm PRIVATE -fexceptions)
  target_link_options(specula-wasm PRIVATE
    --bind
    -sMODULARIZE=1
    -sEXPORT_ES6=1
    -sALLOW_MEMORY_GROWTH=1
    -sENVIRONMENT=web,worker
    -sEXPORTED_RUNTIME_METHODS=['UTF8ToString','lengthBytesUTF8','stringToUTF8']
    -sEXPORTED_FUNCTIONS=['_malloc','_free']
    -sASSERTIONS=1
    -sDISABLE_EXCEPTION_CATCHING=0
  )
  set_target_properties(specula-wasm PROPERTIES
    OUTPUT_NAME lexer
    RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/wasm"
    ARCHIVE_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/wasm"
    LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/wasm"
  )
endif()

add_subdirectory(tests)
