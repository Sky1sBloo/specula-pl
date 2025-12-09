import type { LexerPayload } from '../types/token';

export type LexerWasmModule = {
  tokenize: (source: string) => string | LexerPayload;
};

let modulePromise: Promise<LexerWasmModule> | null = null;

const resolveModule = async (): Promise<LexerWasmModule> => {
  if (!modulePromise) {
    const baseUrl = new URL(import.meta.env.BASE_URL ?? '/', window.location.origin);
    const moduleUrl = new URL('wasm/lexer.js', baseUrl).toString();
    modulePromise = import(/* @vite-ignore */ moduleUrl)
      .then((factory: any) => factory.default ?? factory)
      .then((createModule: any) => createModule());
  }
  return modulePromise;
};

export const tokenizeSource = async (source: string): Promise<LexerPayload> => {
  const module = await resolveModule();
  try {
    const result = module.tokenize(source);
    if (typeof result === 'string') {
      return JSON.parse(result) as LexerPayload;
    }
    return result as LexerPayload;
  } catch (error) {
    const detail = error instanceof Error ? error.message : 'Lexer runtime failure.';
    throw new Error(`Unable to tokenize source: ${detail}`);
  }
};
