# REST API Integration - Quick Start Guide

## Overview

The Specula lexer has been converted from WebAssembly to a REST API backend. The frontend now makes HTTP requests to a Node.js/Express server that calls the C++ lexer executable.

## Setup Instructions

### 1. Install Backend Dependencies

```bash
cd backend
npm install
```

### 2. Ensure C++ Lexer is Built

```bash
# From project root
cmake -B build
cmake --build ./build
```

The executable should be at: `build\Debug\specula.exe`

### 3. Start the Backend Server

```bash
cd backend
npm start
```

You should see:
```
Lexer API server running on http://localhost:3001
Lexer executable: C:\Users\gav\Documents\specula-pl\build\Debug\specula.exe
```

### 4. Start the Frontend

In a separate terminal:

```bash
cd frontend
npm run dev
```

### 5. Test the Integration

Open your browser to the Vite dev server URL (typically `http://localhost:5173`) and try tokenizing some Specula code.

## API Endpoint

**POST** `http://localhost:3001/lex`

**Request Body:**
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

## Testing with curl

```bash
curl -X POST http://localhost:3001/lex -H "Content-Type: application/json" -d "{\"code\":\"let x: int = 5;\"}"
```

## What Changed

### Backend (New)
- `backend/package.json` - Node.js project configuration
- `backend/server.js` - Express server with /lex endpoint
- `backend/.gitignore` - Ignore node_modules and temp files
- `backend/README.md` - Backend documentation

### Frontend (Modified)
- `frontend/src/lib/lexer.ts` - Replaced WebAssembly with REST API calls
- Removed `frontend/src/types/emscripten.d.ts` - No longer needed

### How It Works

1. Frontend sends source code to `POST http://localhost:3001/lex`
2. Backend writes code to temporary file (e.g., `tmp_abc123.txt`)
3. Backend executes: `specula.exe tmp_abc123.txt`
4. Lexer generates: `tmp_abc123_tokens.txt` with JSON output
5. Backend reads JSON and returns it to frontend
6. Backend deletes both temporary files
7. Frontend displays tokens/errors

## Troubleshooting

**"Failed to execute lexer"**
- Check that `build\Debug\specula.exe` exists
- Rebuild the C++ lexer if needed

**CORS errors**
- Ensure backend is running on port 3001
- Check browser console for specific error messages

**Connection refused**
- Make sure the backend server is running
- Verify the frontend is trying to connect to `http://localhost:3001/lex`

## Example Test Code

Try this Specula contract code in the frontend:

```specula
contract Treasury {
  export const owner: Address = caller;
  export const vault: Map<Address, Int>;

  fn deposit(amount: Int) -> Bool {
    require(amount > 0);
    vault[caller] += amount;
    emit Deposited(caller, amount);
    return true;
  }

  fn balance(of: Address) -> Int {
    return vault[of];
  }
}
```
