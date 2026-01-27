import type { LexerPayload } from '../types/token';

const API_URL = import.meta.env.VITE_LEXER_API_URL || 'http://localhost:3001/lex';

/**
 * Tokenize source code by calling the REST API backend
 */
export const tokenizeSource = async (source: string): Promise<LexerPayload> => {
  try {
    const response = await fetch(API_URL, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ code: source }),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ error: 'Unknown error' }));
      throw new Error(errorData.error || `HTTP error ${response.status}`);
    }

    const result = await response.json();

    // Transform backend response to match LexerPayload interface
    // Backend returns: { file, errors, tokens }
    // Frontend expects: { ok, tokens, diagnostics, error }
    const payload: LexerPayload = {
      ok: true,  
      tokens: result.tokens || [],
      // Format diagnostics with location info for proper display
      diagnostics: result.errors?.map((err: any) => 
        `${err.message} at ${err.line}:${err.charPos}`
      ) || [],
      error: result.errors && result.errors.length > 0 ? {
        message: result.errors[0].message,
        line: result.errors[0].line,
        char: result.errors[0].charPos
      } : undefined
    };

    return payload;
  } catch (error) {
    const detail = error instanceof Error ? error.message : 'Lexer API request failed.';
    throw new Error(`Unable to tokenize source: ${detail}`);
  }
};
