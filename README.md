# Specula
## Description
See the private repo for the language description

## Requirements
1. C++
2. CMake
3. gcc/clang (native build)
4. [Emscripten](https://emscripten.org/) (WebAssembly build, optional)
5. Node.js 18+
6. npm 9+

## Installation
### Terminal only 
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

### With Web
1. Install packages
```
npm install
```
2. Build the project
```
npm run build:lexer
```
3. Run development environment
```
npm run dev
```

> You can run `npm run build` to build the website


## Running Tests
The tests are located in the `tests` folder and the build files in the `build/tests` folder. To run it:
```
./build/tests/specula
```
