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
}

export interface LexerPayload {
  ok: boolean;
  tokens: LexerToken[];
  diagnostics?: string[];
  error?: string;
}
