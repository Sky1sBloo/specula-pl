import { useCallback, useEffect, useLayoutEffect, useMemo, useRef, useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { tokenizeSource } from './lib/lexer';
import type { ChangeEvent } from 'react';
import type { LexerPayload, LexerToken } from './types/token';
import * as Styles from './css/Styles';

import OutputBadge from './components/OutputBadge';
import TokenRow from './components/TokenRow';
import OutputEmpty from './components/OutputEmpty';
import OutputErrorPanel from './components/OutputErrorPanel';
import LoadingIndicator from './components/LoadingIndicator';

export const exampleSnippet = `contract Treasury {
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
}`;

export const navItems = ['Compiler', 'About', 'Overview'];

type OutputView = 'all' | 'table' | 'block';

type BlockSummary = {
    id: number;
    startLine?: number;
    endLine?: number;
    label?: string;
    snippet: string[];
    tokens: LexerToken[];
};

const segmentedOptions: { id: OutputView; label: string }[] = [
    { id: 'all', label: 'All' },
    { id: 'table', label: 'Table' },
    { id: 'block', label: 'Block' }
];

// Group contiguous non-empty lines so we can visualize lexing blocks.
const buildBlockSummaries = (source: string, tokens: LexerToken[]): BlockSummary[] => {
    const lines = source.split(/\r?\n/);
    const ranges: Array<{ startLine: number; endLine: number }> = [];
    let blockStart: number | null = null;

    lines.forEach((line, index) => {
        const lineNumber = index + 1;
        if (line.trim()) {
            if (blockStart === null) {
                blockStart = lineNumber;
            }
        } else if (blockStart !== null) {
            ranges.push({ startLine: blockStart, endLine: lineNumber - 1 });
            blockStart = null;
        }
    });

    if (blockStart !== null) {
        ranges.push({ startLine: blockStart, endLine: lines.length });
    }

    const summaries: BlockSummary[] = ranges.map((range, index) => ({
        id: index + 1,
        startLine: range.startLine,
        endLine: range.endLine,
        snippet: lines.slice(range.startLine - 1, range.endLine),
        tokens: []
    }));

    const unmapped: LexerToken[] = [];
    tokens.forEach((token) => {
        if (token.line == null) {
            unmapped.push(token);
            return;
        }

        const block = summaries.find(
            (summary) =>
                summary.startLine != null &&
                summary.endLine != null &&
                token.line! >= summary.startLine &&
                token.line! <= summary.endLine
        );

        if (block) {
            block.tokens.push(token);
        } else {
            unmapped.push(token);
        }
    });

    if (unmapped.length) {
        summaries.push({
            id: summaries.length + 1,
            snippet: ['// residual tokens that could not be mapped to a block'],
            label: 'Residual',
            tokens: unmapped
        });
    }

    return summaries;
};

const buildLineStartIndex = (source: string): number[] => {
    const starts = [0];
    for (let i = 0; i < source.length; i += 1) {
        if (source[i] === '\n') {
            starts.push(i + 1);
        }
    }
    return starts;
};

const resolveLineColumn = (offset: number, starts: number[]) => {
    let low = 0;
    let high = starts.length - 1;
    while (low <= high) {
        const mid = Math.floor((low + high) / 2);
        if (starts[mid] <= offset) {
            low = mid + 1;
        } else {
            high = mid - 1;
        }
    }
    const lineIndex = Math.max(high, 0);
    return {
        line: lineIndex + 1,
        column: offset - starts[lineIndex] + 1
    };
};

const offsetFromLineColumn = (
    line: number | undefined,
    column: number | undefined,
    starts: number[]
) => {
    if (line == null || line < 1 || line > starts.length) {
        return null;
    }
    const base = starts[line - 1];
    const columnOffset = column != null ? Math.max(column - 1, 0) : 0;
    return base + columnOffset;
};

const annotateTokensWithLines = (source: string, tokens: LexerToken[]): LexerToken[] => {
    if (!tokens.length) {
        return [];
    }

    const starts = buildLineStartIndex(source);
    const text = source;
    let cursor = 0;

    return tokens.map((original) => {
        const token = { ...original };
        const lexeme = token.value ?? '';

        if (token.line != null || !lexeme.length) {
            if (lexeme.length) {
                const anchoredOffset = offsetFromLineColumn(token.line, token.column, starts);
                if (anchoredOffset != null) {
                    cursor = anchoredOffset + lexeme.length;
                } else {
                    const existingIndex = text.indexOf(lexeme, cursor);
                    if (existingIndex !== -1) {
                        cursor = existingIndex + lexeme.length;
                    }
                }
            }
            return token;
        }

        const matchIndex = text.indexOf(lexeme, cursor);
        if (matchIndex === -1) {
            return token;
        }

        const position = resolveLineColumn(matchIndex, starts);
        token.line = position.line;
        token.column = position.column;
        cursor = matchIndex + lexeme.length;
        return token;
    });
};

export default function App() {
    const [source, setSource] = useState(exampleSnippet);
    const [payload, setPayload] = useState<LexerPayload | null>(null);
    const [statusText, setStatusText] = useState('Ready to lex. Press Run to analyze.');
    const [errorText, setErrorText] = useState<string | null>(null);
    const [outputView, setOutputView] = useState<OutputView>('all');
    const [editorHeight, setEditorHeight] = useState(280);
    const fileInputRef = useRef<HTMLInputElement | null>(null);
    const textAreaRef = useRef<HTMLTextAreaElement | null>(null);
    const lineNumberRef = useRef<HTMLDivElement | null>(null);
    const [hasRun, setHasRun] = useState(false);
    const [lastRunError, setLastRunError] = useState<string | null>(null);
    const [lastSubmittedSource, setLastSubmittedSource] = useState('');

    useEffect(() => {
        document.body.style.margin = '0';
        document.body.style.backgroundColor = Styles.palette.midnight;
        document.body.style.fontFamily = "'Space Grotesk', 'Segoe UI', sans-serif";
        document.body.style.minHeight = '100vh';
        return () => {
            document.body.removeAttribute('style');
        };
    }, []);

    useEffect(() => {
        const styleElement = document.createElement('style');
        styleElement.textContent = `
      *::-webkit-scrollbar { width: 8px; height: 8px; }
      *::-webkit-scrollbar-track { background: transparent; }
      *::-webkit-scrollbar-thumb {
        background: #0b1a44;
        border-radius: 999px;
        border: 2px solid transparent;
        background-clip: padding-box;
      }
      * {
        scrollbar-color: #0b1a44 transparent;
        scrollbar-width: thin;
      }
      @keyframes spin {
        from { transform: rotate(0deg); }
        to { transform: rotate(360deg); }
      }
    `;
        document.head.appendChild(styleElement);
        return () => {
            document.head.removeChild(styleElement);
        };
    }, []);

    const lineNumberText = useMemo(() => {
        const lines = source.split(/\r?\n/);
        const digits = String(Math.max(lines.length, 1)).length;
        return lines.map((_, idx) => String(idx + 1).padStart(digits, ' ')).join('\n');
    }, [source]);

    const syncLineScroll = useCallback(() => {
        if (lineNumberRef.current && textAreaRef.current) {
            lineNumberRef.current.scrollTop = textAreaRef.current.scrollTop;
        }
    }, []);

    useEffect(() => {
        syncLineScroll();
    }, [lineNumberText, syncLineScroll]);

    const measureEditorHeight = useCallback(() => {
        if (!textAreaRef.current) {
            return;
        }
        const textarea = textAreaRef.current;
        textarea.style.height = 'auto';
        const nextHeight = Math.max(textarea.scrollHeight, 200);
        setEditorHeight(nextHeight);
    }, []);

    useLayoutEffect(() => {
        measureEditorHeight();
    }, [measureEditorHeight, source]);

    useEffect(() => {
        window.addEventListener('resize', measureEditorHeight);
        return () => {
            window.removeEventListener('resize', measureEditorHeight);
        };
    }, [measureEditorHeight]);


    const lexerMutation = useMutation({
        mutationFn: tokenizeSource,
        onSuccess: (data) => {
            console.log(data)
            if (!data) {
                const emptyMessage = 'Lexer returned an empty response.';
                setPayload(null);
                setStatusText('Lexing failed');
                setErrorText(emptyMessage);
                setLastRunError(emptyMessage);
                return;
            }

            const rawTokens = (data as { tokens?: LexerToken[] }).tokens ?? [];
            const normalizedTokens = Array.isArray(rawTokens) ? rawTokens : [];
            const normalizedPayload: LexerPayload = {
                ok: !!data.ok,
                tokens: normalizedTokens,
                diagnostics: data.diagnostics,
                error: data.error
            };
            setPayload(normalizedPayload);
            const tokenCount = normalizedTokens.length;
            const descriptor = tokenCount === 1 ? 'token' : 'tokens';
            setStatusText(
                normalizedPayload.ok
                    ? `Lexing complete • ${tokenCount} ${descriptor}`
                    : 'Lexer reported diagnostics'
            );
            if (!normalizedPayload.ok) {
                if (!normalizedPayload.error) {
                    setErrorText("Unexpected lexer issue");
                } else {
                    setErrorText(`Error at: ${normalizedPayload.error.line}:${normalizedPayload.error.char}\n Error: ${normalizedPayload.error.message}`);
                }
            }
            setLastRunError(null);
        },
        onError: (err: unknown) => {
            const message = err instanceof Error ? err.message : 'Unknown lexer failure.';
            setErrorText(message);
            setStatusText('Lexing failed');
            setLastRunError(message);
            setPayload(null);
        }
    });

    useEffect(() => {
        if (!hasRun || lexerMutation.isPending) {
            return;
        }
        if (source !== lastSubmittedSource) {
            setStatusText('Source modified since last run. Press Run to re-analyze.');
        }
    }, [hasRun, source, lastSubmittedSource, lexerMutation.isPending]);

    const runLex = useCallback(
        (input?: string) => {
            const code = input ?? source;
            if (!code.trim()) {
                setPayload(null);
                setStatusText('Waiting for Specula input…');
                setErrorText(null);
                setHasRun(false);
                setLastSubmittedSource('');
                return;
            }

            setStatusText('Lexing Specula source…');
            setErrorText(null);
            setHasRun(true);
            setLastRunError(null);
            setLastSubmittedSource(code);
            lexerMutation.mutate(code);
        },
        [lexerMutation, source]
    );

    const tokenList = useMemo(() => {
        const tokens = payload?.tokens ?? [];
        return annotateTokensWithLines(source, tokens);
    }, [payload, source]);
    const blockSummaries = useMemo(() => buildBlockSummaries(source, tokenList), [source, tokenList]);

    const handleRunClick = () => {
        runLex();
    };

    const handleNewClick = () => {
        fileInputRef.current?.click();
    };

    const handleFileSelected = (event: ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (!file) {
            return;
        }

        const reader = new FileReader();
        reader.onload = () => {
            if (typeof reader.result === 'string') {
                setSource(reader.result);
                setStatusText(`Loaded ${file.name}. Press Run to analyze.`);
                setHasRun(false);
                setPayload(null);
                setLastRunError(null);
                setLastSubmittedSource('');
            } else {
                setErrorText('Unsupported file encoding.');
            }
        };
        reader.onerror = () => {
            setErrorText('Unable to read the selected file.');
        };
        reader.readAsText(file);

        event.target.value = '';
    };

    const handleEditorScroll = () => {
        syncLineScroll();
    };

    const renderAllTokens = () => (
        <div style={{ ...Styles.outputScroll, minHeight: editorHeight, maxHeight: editorHeight }}>
            {tokenList.map((token, index) => (
                <TokenRow token={token} key={`${token.type}-${token.value}-${index}`} />
            ))}
        </div>
    );

    const renderTableTokens = () => (
        <div style={{ ...Styles.tableWrapperStyle, minHeight: editorHeight, maxHeight: editorHeight }}>
            <table style={{ width: '100%', borderCollapse: 'separate', borderSpacing: 0 }}>
                <thead>
                    <tr>
                        <th style={Styles.tableHeaderCellStyle}>Lexeme</th>
                        <th style={Styles.tableHeaderCellStyle}>Token</th>
                        <th style={{ ...Styles.tableHeaderCellStyle, textAlign: 'right' }}>Line</th>
                    </tr>
                </thead>
                <tbody>
                    {tokenList.map((token, index) => (
                        <tr key={`${token.type}-${token.value}-${index}`}>
                            <td style={{ ...Styles.tableCellStyle, color: Styles.palette.frost }}>{token.value}</td>
                            <td style={Styles.tableCellStyle}>{token.type}</td>
                            <td style={{ ...Styles.tableCellStyle, textAlign: 'right', color: Styles.palette.muted }}>
                                {token.line ?? '—'}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );

    const renderBlockTokens = () => {
        if (!blockSummaries.length) {
            return <OutputEmpty height={editorHeight} message="Unable to segment this source into blocks." />;
        }

        return (
            <div
                style={{
                    ...Styles.outputScroll,
                    paddingRight: '6px',
                    minHeight: editorHeight,
                    maxHeight: editorHeight
                }}
            >
                <div
                    style={{
                        fontSize: '0.78rem',
                        textTransform: 'uppercase',
                        letterSpacing: '0.2em',
                        color: Styles.palette.muted
                    }}
                >
                    Total Blocks: {blockSummaries.length}
                </div>
                {blockSummaries.map((block) => (
                    <div key={block.id} style={Styles.blockCardStyle}>
                        <div style={Styles.blockHeaderStyle}>
                            <span>{block.label ?? `Block ${block.id}`}</span>
                            <span>
                                {block.startLine != null && block.endLine != null
                                    ? `Lines ${block.startLine}–${block.endLine}`
                                    : 'No line data'}
                            </span>
                            <span>{block.tokens.length} tokens</span>
                        </div>
                        <pre style={Styles.blockCodeStyle}>{block.snippet.join('\n') || '// empty block'}</pre>
                        {block.tokens.length ? (
                            <div style={Styles.chipTrayStyle}>
                                {block.tokens.map((token, index) => (
                                    <span key={`${block.id}-${token.type}-${token.value}-${index}`} style={Styles.chipStyle}>
                                        <span style={Styles.chipTypeStyle}>{token.type}</span>
                                        <span>{token.value}</span>
                                    </span>
                                ))}
                            </div>
                        ) : (
                            <div style={{ color: Styles.palette.muted, fontSize: '0.78rem' }}>
                                No tokens in this block.
                            </div>
                        )}
                    </div>
                ))}
            </div>
        );
    };

    const renderTokensByView = () => {
        switch (outputView) {
            case 'table':
                return renderTableTokens();
            case 'block':
                return renderBlockTokens();
            default:
                return renderAllTokens();
        }
    };

    const renderOutput = () => {
        if (!hasRun) {
            return <OutputEmpty height={editorHeight} message="// Press Run to analyze your Specula code" />;
        }

        if (lexerMutation.isPending) {
            return <LoadingIndicator height={editorHeight} />;
        }

        if (lastRunError) {
            return <OutputErrorPanel height={editorHeight} message={lastRunError} diagnostics={payload?.diagnostics} />;
        }

        if (!payload) {
            return <OutputEmpty height={editorHeight} message="// Lexical analysis output will appear here" />;
        }

        if (!payload.ok) {
            return (
                <OutputErrorPanel
                    height={editorHeight}
                    message={payload.error ? payload.error.message : 'The lexer reported issues. Check your syntax and try again.'}
                    diagnostics={payload.diagnostics}
                />
            );
        }

        if (!tokenList.length) {
            return <OutputEmpty height={editorHeight} message="No tokens produced yet." />;
        }

        return renderTokensByView();
    };

    return (
        <div style={Styles.appShell}>
            <nav style={Styles.navStyles}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '10px', justifySelf: 'start' }}>
                    <img src="/specula-pl/wasm/assets/specula-logo_med.png" alt="Specula" style={{ height: '32px', width: 'auto' }} />
                </div>
                <div style={Styles.navLinksStyle}>
                    <p style={{ marginLeft: '10px' }}>Stateful  Contracts  Language  for  Safe  Systems</p>
                </div>
                {/* <div style={{ color: Styles.palette.muted, fontSize: '0.9rem', justifySelf: 'end', textAlign: 'right' }}>
                    v1.0
                </div> */}
            </nav>

            <section style={Styles.heroCopyStyle}>

                <img src="/specula-pl/wasm/assets/specula.png" alt="Specula" style={{ maxWidth: '600px', height: 'auto' }} />
                {/* <p style={heroLeadStyle}>
                    Lorem Ipsum Dolor Sit Amet, Consectetur Adipiscing Elit. Sed Do Eiusmod Tempor Incididunt Ut
                </p> */}
                <div style={Styles.ctaRowStyle}>
                    <button style={Styles.primaryButton} onClick={() => setSource(exampleSnippet)}>
                        Load Example
                    </button>
                    <a
                        href="https://github.com/Sky1sBloo/specula-pl"
                        target="_blank"
                        rel="noreferrer"
                        style={{ textDecoration: 'none' }}
                    >
                        <button style={Styles.secondaryButton}>Learn Specula</button>
                    </a>
                </div>
            </section>

            <input
                ref={fileInputRef}
                type="file"
                accept=".spec,.txt,.sol,.rs,.json,.ts,.js,.c,.cpp,.md,.wasm,.wat,.wl"
                style={{ display: 'none' }}
                onChange={handleFileSelected}
            />
            <section style={Styles.studioLayout}>
                <div style={Styles.panelChrome}>
                    <div style={Styles.panelTitleRow}>
                        <div style={{ fontWeight: 600 }}>Code Editor</div>
                        <div style={Styles.tabRowStyle}>
                            <button
                                style={{
                                    ...Styles.runButtonStyle,
                                    opacity: lexerMutation.isPending ? 0.6 : 1,
                                    cursor: lexerMutation.isPending ? 'wait' : 'pointer'
                                }}
                                onClick={handleRunClick}
                                disabled={lexerMutation.isPending}
                            >
                                Run
                            </button>
                            <button style={Styles.newButtonStyle} onClick={handleNewClick}>
                                New
                            </button>
                        </div>
                    </div>
                    <div style={{ ...Styles.editorShellStyle, height: editorHeight }}>
                        <div ref={lineNumberRef} style={{ ...Styles.lineNumberColumnStyle, height: editorHeight }}>
                            <pre style={Styles.lineNumberInnerStyle} aria-hidden="true">
                                {lineNumberText}
                            </pre>
                        </div>
                        <textarea
                            ref={textAreaRef}
                            spellCheck={false}
                            style={{ ...Styles.editorStyle, height: editorHeight }}
                            value={source}
                            onScroll={handleEditorScroll}
                            onChange={(evt) => setSource(evt.target.value)}
                            placeholder="// Start typing Specula code…"
                        />
                    </div>
                    <div style={Styles.statusTextStyle}>{statusText}</div>
                    {errorText && (
                        <div style={{ ...Styles.statusTextStyle, color: '#ff7b7b' }}>{errorText}</div>
                    )}
                </div>

                <div style={Styles.panelChrome}>
                    <div style={{ ...Styles.panelTitleRow, flexWrap: 'wrap', gap: '12px' }}>
                        <div style={{ fontWeight: 600, display: 'flex', alignItems: 'center', gap: '8px' }}>
                            Output{' '}
                            <OutputBadge
                                ok={
                                    hasRun
                                        ? !lastRunError && (payload ? !!payload.ok : true)
                                        : true
                                }
                            />
                        </div>
                        <div
                            style={{
                                display: 'flex',
                                alignItems: 'center',
                                gap: '12px',
                                flexWrap: 'wrap',
                                justifyContent: 'flex-end'
                            }}
                        >
                            <div style={Styles.segmentedRowStyle}>
                                {segmentedOptions.map((option) => {
                                    const isActive = outputView === option.id;
                                    return (
                                        <button
                                            key={option.id}
                                            type="button"
                                            style={{
                                                ...Styles.segmentedButtonBase,
                                                background: isActive ? 'rgba(63,115,255,0.4)' : 'transparent',
                                                color: isActive ? '#fdfdff' : Styles.palette.muted,
                                                boxShadow: isActive ? '0 6px 20px rgba(63,115,255,0.35)' : 'none'
                                            }}
                                            onClick={() => setOutputView(option.id)}
                                        >
                                            {option.label}
                                        </button>
                                    );
                                })}
                            </div>
                            <button
                                style={{
                                    ...Styles.buttonBase,
                                    padding: '8px 18px',
                                    borderRadius: '12px',
                                    background: 'rgba(255,255,255,0.08)',
                                    color: Styles.palette.frost,
                                    border: '1px solid rgba(255,255,255,0.12)'
                                }}
                                onClick={() => payload && navigator.clipboard?.writeText(JSON.stringify(payload, null, 2))}
                            >
                                Copy JSON
                            </button>
                        </div>
                    </div>
                    {renderOutput()}
                </div>
            </section>
        </div>
    );
}
