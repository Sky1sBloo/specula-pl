import type { LexerPayload, LexerToken } from '../types/token';

// Use the Node.js backend as a proxy to avoid CORS issues
const PARSER_API_URL = import.meta.env.VITE_PARSER_API_URL || 'http://localhost:3001/parse';

export interface ParserPayload {
  ok: boolean;
  fileInfo?: {
    name: string;
    type: string;
  };
  errors: string[];
  root: unknown | null;
}

/**
 * Send lexer output to the parser backend for syntax analysis
 */
export const parseTokens = async (
  tokens: LexerToken[],
  errors: Array<{ line: number; charPos: number; message: string }> = []
): Promise<ParserPayload> => {
  try {
    // Format the request to match what the C# backend expects (LexerOutput)
    // The lexer already outputs tokens in the correct format:
    // { type: string, value: string, line: number, char_start: number, char_end: number }
    const requestBody = {
      file: {
        name: 'editor.spec',
        type: 'specula'
      },
      errors: errors,
      tokens: tokens
    };

    const response = await fetch(PARSER_API_URL, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(requestBody),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ error: 'Unknown error' }));
      throw new Error(errorData.error || `HTTP error ${response.status}`);
    }

    const result = await response.json();

    // Transform backend response to match ParserPayload interface
    // C# Backend returns: { fileInfo, errors, root }
    const payload: ParserPayload = {
      ok: !result.errors || result.errors.length === 0,
      fileInfo: result.fileInfo,
      errors: result.errors || [],
      root: result.root
    };

    return payload;
  } catch (error) {
    const detail = error instanceof Error ? error.message : 'Parser API request failed.';
    throw new Error(`Unable to parse tokens: ${detail}`);
  }
};

/**
 * Run the full pipeline: tokenize source, then parse tokens
 * This calls the lexer first, then the parser
 */
export const analyzeSource = async (
  lexerPayload: LexerPayload
): Promise<ParserPayload> => {
  if (!lexerPayload.ok || !lexerPayload.tokens.length) {
    return {
      ok: false,
      errors: lexerPayload.diagnostics || ['Lexer failed - cannot parse'],
      root: null
    };
  }

  // Convert lexer errors to the format expected by parser
  const lexerErrors = lexerPayload.error
    ? [{
      line: lexerPayload.error.line,
      charPos: lexerPayload.error.char,
      message: lexerPayload.error.message
    }]
    : [];

  return parseTokens(lexerPayload.tokens, lexerErrors);
};
