import type { LexerPayload } from '../types/token';

export type LexerWasmModule = {
  tokenize: (source: string) => string | LexerPayload;
};

let modulePromise: Promise<LexerWasmModule> | null = null;

const resolveModule = async (): Promise<LexerWasmModule> => {
  if (!modulePromise) {
    const moduleUrl = new URL('/wasm/lexer.js', window.location.origin).toString();
    modulePromise = import(/* @vite-ignore */ moduleUrl)
      .then((factory: any) => factory.default ?? factory)
      .then((createModule: any) => createModule());
  }
  return modulePromise;
};

export const tokenizeSource = async (source: string): Promise<LexerPayload> => {
  const module = await resolveModule();
  const result = module.tokenize(source);
  if (typeof result === 'string') {
    return JSON.parse(result) as LexerPayload;
  }
  return result as LexerPayload;
};
