# Specula
## Description
See the private repo for the language description

## Requirements
1. C++
2. CMake
3. gcc/clang (native build)
4. [Emscripten](https://emscripten.org/) (WebAssembly build, optional)

## Installation
1. Build cmake files
```
cmake -B build
```
2. Build the project
```
cmake --build ./build
```
3. Run the project
```
./build/specula [file1] [file2] ...
```
4. Each processed file produces a JSON payload at
```
<original>_tokens<extension>
```
with the format
```json
{
	"ok": true,
	"tokens": [
		{ "type": "IDENTIFIER", "value": "foo" }
	]
}
```
## Running Tests
The tests are located in the `tests` folder and the build files in the `build/tests` folder. To run it:
```
./build/tests/specula
```

## Building the WebAssembly Module
1. Install Emscripten and activate the SDK environment (`emsdk_env` script).
2. Configure the project with the Emscripten toolchain and enable the wasm target:
```
emcmake cmake -B build-wasm -DSPECULA_BUILD_WASM=ON
```
3. Build the module:
```
cmake --build build-wasm
```
4. The artifacts `lexer.wasm` and `lexer.js` are emitted under `build-wasm/wasm/` and expose a single embind function `tokenize(source: string)` that returns the JSON payload shown above.

## Frontend UI (React)
The GitHub Pages-ready interface lives in `frontend/` and mirrors the provided SynTA mockup with a dual-panel layout (script editor + code-block inspector).

### Prerequisites
- Node.js 18+
- npm 9+

### Local Development
```
cd frontend
npm install        # needed once to sync dependencies after pulling
npm run dev        # starts Vite on http://localhost:5173
```

### Production Build
```
cd frontend
npm run build      # outputs to frontend/dist
```
Serve the `dist/` folder via any static host (GitHub Pages, Netlify, etc.).

