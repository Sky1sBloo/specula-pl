export type TokenKind =
  | 'IDENTIFIER'
  | 'KEYWORD'
  | 'NUMBER'
  | 'STRING'
  | 'SYMBOL'
  | 'COMMENT'
  | 'WHITESPACE'
  | string;

export interface LexerToken {
  type: TokenKind;
  value: string;
  line?: number;
  column?: number;
  // Fields from C++ lexer for parser compatibility
  char_start?: number;
  char_end?: number;
}

export interface LexerPayload {
  ok: boolean;
  tokens: LexerToken[];
  diagnostics?: string[];
  error?: LexerError;
}

export interface LexerError {
    message: string;
    line: number;
    char: number;
}
