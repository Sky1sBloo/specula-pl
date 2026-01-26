# Specula-PL: Running the Full Stack

This guide explains how to run the complete Specula compiler frontend with lexer and parser backends.

## Prerequisites

- **Node.js** (v18+)
- **.NET 10** SDK
- **CMake** (for building the C++ lexer)
- **C++ Compiler** (clang/gcc)

## Architecture

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────────┐
│  Vite Frontend  │────▶│  Node.js Backend │────▶│  C# Parser Backend  │
│  (React + TS)   │     │  (Express API)   │     │  (ASP.NET Core)     │
│  Port: 5173     │     │  Port: 3001      │     │  Port: 5000         │
└─────────────────┘     └────────┬─────────┘     └─────────────────────┘
                                 │
                                 ▼
                        ┌─────────────────┐
                        │  C++ Lexer      │
                        │  (executable)   │
                        └─────────────────┘
```

## Setup & Run Instructions

### 1. Build the C++ Lexer

```bash
cd specula-pl
mkdir -p build && cd build
cmake ..
cmake --build . --config Debug
cd ..
```

This creates the lexer executable at `build/specula`.

### 2. Start the C# Parser Backend

Open a **new terminal**:

```bash
cd specula-syntax-analyzer/SpeculaSyntaxAnalyzer
dotnet run
```

You should see:
```
Now listening on: http://localhost:5000
Application started.
```

### 3. Start the Node.js Backend

Open a **new terminal**:

```bash
cd specula-pl/backend
npm install   # First time only
npm start
```

You should see:
```
Lexer API server running on http://localhost:3001
Lexer executable: /path/to/specula-pl/build/specula
Parser API URL: http://localhost:5000/api/analyze
```

### 4. Start the Frontend

Open a **new terminal**:

```bash
cd specula-pl/frontend
npm install   # First time only
npm run dev
```

You should see:
```
VITE ready
Local: http://localhost:5173/specula-pl/
```

### 5. Open the Application

Navigate to: **http://localhost:5173/specula-pl/**

## Usage

1. **Lexer Mode**: Tokenizes your Specula source code
   - Views: All, Table, Block, JSON

2. **Parser Mode**: Parses tokens into an AST
   - Views: Tree, Dropdown, JSON
   - Requires valid Specula syntax

## Example Valid Specula Code

```specula
contract MotionControl {
    init-state Idle;
    roles: Robot, Controller;

    state Idle -> CommandSent -> Acknowledged -> Completed;

    [Controller -> Robot]
    MoveCommand { direction: str, speed: float } @Idle -> CommandSent;
    
    [Robot -> Controller]
    Ack { accepted: bool } @CommandSent -> Acknowledged;

    fail low_battery; 
    auto-reset after Completed;
}
```

## Troubleshooting

### Lexer not found
Make sure you built the C++ lexer and the path in `backend/server.js` matches your build output.

### Parser connection refused
Ensure the C# backend is running on port 5000.

### CORS errors
The Node.js backend proxies parser requests to avoid CORS issues. Make sure you're calling `/parse` on port 3001, not directly to port 5000.

## API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `http://localhost:3001/lex` | POST | Tokenize source code |
| `http://localhost:3001/parse` | POST | Parse lexer output |
| `http://localhost:3001/health` | GET | Health check |
