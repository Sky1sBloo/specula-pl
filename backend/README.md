# Specula Lexer REST API Backend

This is a Node.js/Express backend that provides a REST API for the Specula C++ lexer. It replaces the previous WebAssembly integration.

## Prerequisites

1. **Node.js** (v18 or higher recommended)
2. **Compiled C++ Lexer** - The lexer executable must be built at `../build/Debug/specula.exe`

## Installation

```bash
cd backend
npm install
```

## Running the Server

```bash
npm start
```

The server will start on `http://localhost:3001` by default.

For development with auto-restart on file changes (requires Node.js v18+):
```bash
npm run dev
```

## API Endpoints

### POST /lex

Tokenizes Specula source code.

**Request:**
```json
{
  "code": "let x: int = 5;"
}
```

**Response:**
```json
{
  "file": {
    "name": "tmp_abc123",
    "type": "specula_src"
  },
  "errors": [],
  "tokens": [
    {
      "type": "K_LET",
      "value": "let",
      "char_start": 1,
      "char_end": 4,
      "line": 1
    }
  ]
}
```

**Error Response:**
```json
{
  "error": "Error message describing what went wrong"
}
```

### GET /health

Health check endpoint.

**Response:**
```json
{
  "status": "ok",
  "message": "Lexer API is running"
}
```

## Testing the API

You can test the API using curl:

```bash
curl -X POST http://localhost:3001/lex -H "Content-Type: application/json" -d "{\"code\":\"let x: int = 5;\"}"
```

## Integration with Frontend

The Vite frontend should make POST requests to `http://localhost:3001/lex` with the source code in the request body.

Example using fetch:
```javascript
const response = await fetch('http://localhost:3001/lex', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ code: sourceCode })
});
const result = await response.json();
```

## How It Works

1. Frontend sends source code via POST request
2. Backend writes code to a temporary file (e.g., `tmp_abc123.txt`)
3. Backend executes the C++ lexer: `specula.exe tmp_abc123.txt`
4. Lexer generates output file: `tmp_abc123_tokens.txt`
5. Backend reads and parses the JSON output
6. Backend returns JSON to frontend
7. Backend cleans up both temporary files

## Configuration

You can customize the port by setting the `PORT` environment variable:

```bash
PORT=4000 npm start
```

## Troubleshooting

**Error: "Failed to execute lexer"**
- Ensure the C++ lexer is compiled at `../build/Debug/specula.exe`
- Check that the executable has proper permissions

**Error: "Lexer did not generate output file"**
- The lexer may have crashed or encountered an error
- Check the console output for stderr messages from the lexer

**CORS errors in browser**
- The server is configured to allow all origins via CORS
- If you need to restrict origins, modify the `cors()` configuration in `server.js`
