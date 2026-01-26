# Specula Syntax Analyzer API

ASP.NET Core API for analyzing Specula source code tokens and generating an abstract syntax tree (AST).

## Usage

Start the server:
```bash
dotnet run
```

The API listens on `http://localhost:5000`.

## Endpoint

### POST `/api/analyze`

Accepts lexer output (token stream) and returns syntax analysis.

**Input Format:**
```json
{
  "file": {
    "name": "filename",
    "type": "specula_src"
  },
  "tokens": [
    {
      "type": "K_FN",
      "value": "fn",
      "line": 1,
      "char_start": 1,
      "char_end": 3
    }
  ],
  "errors": []
}
```

**Response:**
```json
{
  "fileInfo": {
    "name": "filename",
    "type": "specula_src"
  },
  "errors": [],
  "root": {
    "$type": "RootNode",
    "statements": [...]
  }
}
```

## Example

Request body (`test.json`):
```json
{
  "file": {"name": "test", "type": "specula_src"},
  "tokens": [
    {"type": "K_FN", "value": "fn", "line": 1, "char_start": 1, "char_end": 3},
    {"type": "IDENT", "value": "main", "line": 1, "char_start": 4, "char_end": 8}
  ],
  "errors": []
}
```

Response includes parsed AST with `$type` fields for each node (e.g., `FuncDefNode`, `BodyNode`, etc.).

## Notes

- Input tokens must come from the Specula lexer
- All parse errors are collected in the `errors` array
- Each AST node includes a `$type` field identifying its type
