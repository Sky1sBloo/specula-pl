#!/usr/bin/bash
set -e

BUILD_DIR="build-wasm"
OUTPUT_DIR="$BUILD_DIR/wasm"
DEST="public/wasm"

emcmake cmake -B "$BUILD_DIR" -DSPECULA_BUILD_WASM=ON
cmake --build "$BUILD_DIR"

mkdir -p "$DEST"
cp "$OUTPUT_DIR/lexer.wasm" "$DEST/"
cp "$OUTPUT_DIR/lexer.js" "$DEST/"
