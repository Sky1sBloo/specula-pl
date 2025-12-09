import { useCallback, useEffect, useLayoutEffect, useMemo, useRef, useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { tokenizeSource } from './lib/lexer';
import type { CSSProperties, ChangeEvent } from 'react';
import type { LexerPayload, LexerToken } from './types/token';

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

const palette = {
  midnight: '#01010f',
  ocean: '#041137',
  royal: '#1438ff',
  electric: '#3f73ff',
  frost: 'rgba(255, 255, 255, 0.72)',
  muted: 'rgba(255, 255, 255, 0.45)',
  glass: 'rgba(7, 12, 40, 0.76)'
};

const exampleSnippet = `contract Treasury {
  const owner: Address = caller;
  const vault: Map<Address, Int>;

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

const navItems = ['Compiler', 'About', 'Overview'];

const appShell: CSSProperties = {
  minHeight: '100vh',
  padding: '32px 40px 48px',
  backgroundColor: palette.midnight,
  backgroundImage:
    'radial-gradient(circle at 20% 20%, rgba(31,78,255,0.25), transparent 55%), radial-gradient(circle at 80% 0%, rgba(14,54,166,0.32), transparent 45%), repeating-linear-gradient(-80deg, rgba(4,6,30,0.95) 0px, rgba(4,6,30,0.95) 60px, rgba(7, 24, 65, 0.7) 120px, rgba(5, 16, 51, 0.7) 180px)',
  color: '#f8fbff'
};

const navStyles: CSSProperties = {
  display: 'grid',
  gridTemplateColumns: '1fr auto 1fr',
  alignItems: 'center',
  fontSize: '0.95rem',
  marginBottom: '48px',
  padding: '16px 32px',
  borderRadius: '28px',
  background: 'rgba(4, 8, 30, 0.35)',
  border: '1px solid rgba(255,255,255,0.08)',
  backdropFilter: 'blur(18px)'
};

const navLinksStyle: CSSProperties = {
  display: 'flex',
  gap: '32px',
  color: palette.muted,
  textTransform: 'uppercase',
  fontSize: '0.85rem',
  letterSpacing: '0.2em',
  justifySelf: 'center'
};

const heroCopyStyle: CSSProperties = {
  textAlign: 'center',
  maxWidth: '640px',
  margin: '0 auto 40px'
};

const heroTitleStyle: CSSProperties = {
  fontSize: '3.3rem',
  letterSpacing: '0.08em',
  marginBottom: '16px'
};

const heroLeadStyle: CSSProperties = {
  color: palette.frost,
  lineHeight: 1.6,
  fontSize: '1.05rem'
};

const ctaRowStyle: CSSProperties = {
  display: 'flex',
  justifyContent: 'center',
  gap: '16px',
  marginTop: '28px'
};

const buttonBase: CSSProperties = {
  border: 'none',
  borderRadius: '999px',
  padding: '14px 28px',
  fontSize: '0.95rem',
  cursor: 'pointer',
  fontWeight: 600,
  transition: 'transform 220ms ease, box-shadow 220ms ease'
};

const primaryButton: CSSProperties = {
  ...buttonBase,
  backgroundColor: 'rgba(255, 255, 255, 1)',
  color: '#000000ff',
  boxShadow: '0 10px 30px rgba(60, 44, 199, 0.35)'
};

const secondaryButton: CSSProperties = {
  ...buttonBase,
  backgroundColor: 'rgba(255, 255, 255, 0.1)',
  color: '#fffefeff',
  border: '3px solid rgba(255,255,255,0.18)'
};

const studioLayout: CSSProperties = {
  display: 'grid',
  gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))',
  gap: '24px'
};

const panelChrome: CSSProperties = {
  background: palette.glass,
  borderRadius: '24px',
  border: '1px solid rgba(255,255,255,0.08)',
  padding: '22px',
  minHeight: '360px',
  boxShadow: '0 20px 60px rgba(2,0,20,0.55)',
  backdropFilter: 'blur(16px)'
};

const panelTitleRow: CSSProperties = {
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  marginBottom: '16px'
};

const tabRowStyle: CSSProperties = {
  display: 'flex',
  gap: '8px',
  fontSize: '0.85rem'
};

const editorActionButton: CSSProperties = {
  ...buttonBase,
  padding: '8px 18px',
  borderRadius: '12px',
  background: 'rgba(255,255,255,0.08)',
  color: palette.frost,
  border: '1px solid rgba(255,255,255,0.12)'
};

const runButtonStyle: CSSProperties = {
  ...editorActionButton,
  background: 'rgba(63,115,255,0.2)',
  borderColor: 'rgba(63,115,255,0.6)'
};

const newButtonStyle: CSSProperties = {
  ...editorActionButton,
  background: 'rgba(255,255,255,0.04)',
  borderColor: 'rgba(255,255,255,0.18)'
};

const editorShellStyle: CSSProperties = {
  display: 'grid',
  gridTemplateColumns: 'auto 1fr',
  background: 'rgba(2,6,26,0.85)',
  borderRadius: '18px',
  border: '1px solid rgba(255,255,255,0.08)',
  overflow: 'hidden'
};

const lineNumberColumnStyle: CSSProperties = {
  display: 'flex',
  justifyContent: 'flex-end',
  alignItems: 'flex-start',
  background: 'rgba(1,3,20,0.85)',
  color: palette.muted,
  fontFamily: "'JetBrains Mono', 'Space Grotesk', monospace",
  fontSize: '0.85rem',
  padding: '18px 12px',
  textAlign: 'right',
  borderRight: '1px solid rgba(255,255,255,0.08)',
  minWidth: '52px',
  boxSizing: 'border-box',
  overflow: 'hidden',
  userSelect: 'none'
};

const lineNumberInnerStyle: CSSProperties = {
  margin: 0,
  lineHeight: 1.6,
  whiteSpace: 'pre'
};

const editorStyle: CSSProperties = {
  width: '100%',
  background: 'transparent',
  border: 'none',
  padding: '18px',
  color: palette.frost,
  fontSize: '0.95rem',
  fontFamily: "'JetBrains Mono', 'Space Grotesk', monospace",
  resize: 'none',
  outline: 'none',
  lineHeight: 1.6,
  boxSizing: 'border-box'
};

const statusTextStyle: CSSProperties = {
  fontSize: '0.85rem',
  color: palette.muted,
  marginTop: '12px'
};

const outputScroll: CSSProperties = {
  overflowY: 'auto',
  display: 'flex',
  flexDirection: 'column',
  gap: '10px'
};

const loadingShellStyle: CSSProperties = {
  ...panelChrome,
  minHeight: 200,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center'
};

const loadingSpinnerStyle: CSSProperties = {
  width: '54px',
  height: '54px',
  borderRadius: '50%',
  border: '3px solid rgba(255,255,255,0.45)',
  borderTopColor: 'transparent',
  animation: 'spin 1s linear infinite'
};

const errorPanelStyle: CSSProperties = {
  ...panelChrome,
  display: 'flex',
  flexDirection: 'column',
  gap: '12px',
  alignItems: 'flex-start',
  borderColor: 'rgba(255,123,123,0.3)'
};

const diagnosticBlockStyle: CSSProperties = {
  width: '100%',
  background: 'rgba(255,123,123,0.08)',
  borderRadius: '12px',
  padding: '12px 14px',
  fontSize: '0.85rem',
  color: palette.frost,
  fontFamily: "'JetBrains Mono', 'Space Grotesk', monospace"
};

const segmentedRowStyle: CSSProperties = {
  display: 'grid',
  gridTemplateColumns: 'repeat(3, minmax(0, 1fr))',
  background: 'rgba(255,255,255,0.06)',
  borderRadius: '14px',
  padding: '4px',
  minWidth: '230px'
};

const segmentedButtonBase: CSSProperties = {
  border: 'none',
  borderRadius: '10px',
  padding: '8px 0',
  fontSize: '0.75rem',
  textTransform: 'uppercase',
  letterSpacing: '0.22em',
  background: 'transparent',
  color: palette.muted,
  cursor: 'pointer',
  transition: 'all 160ms ease'
};

const tableWrapperStyle: CSSProperties = {
  overflowY: 'auto',
  overflowX: 'auto',
  borderRadius: '16px',
  border: '1px solid rgba(255,255,255,0.06)'
};

const tableHeaderCellStyle: CSSProperties = {
  textAlign: 'left',
  padding: '8px 12px',
  fontSize: '0.75rem',
  textTransform: 'uppercase',
  letterSpacing: '0.18em',
  color: palette.muted,
  borderBottom: '1px solid rgba(255,255,255,0.12)'
};

const tableCellStyle: CSSProperties = {
  padding: '10px 12px',
  fontSize: '0.85rem',
  borderBottom: '1px solid rgba(255,255,255,0.06)'
};

const blockCardStyle: CSSProperties = {
  padding: '14px',
  borderRadius: '16px',
  background: 'rgba(255,255,255,0.04)',
  border: '1px solid rgba(255,255,255,0.08)',
  display: 'flex',
  flexDirection: 'column',
  gap: '10px'
};

const blockHeaderStyle: CSSProperties = {
  display: 'flex',
  justifyContent: 'space-between',
  flexWrap: 'wrap',
  gap: '6px',
  fontSize: '0.8rem',
  textTransform: 'uppercase',
  letterSpacing: '0.18em',
  color: palette.muted
};

const blockCodeStyle: CSSProperties = {
  margin: 0,
  borderRadius: '12px',
  background: 'rgba(1,1,15,0.8)',
  padding: '10px 12px',
  fontFamily: "'JetBrains Mono', 'Space Grotesk', monospace",
  fontSize: '0.85rem',
  whiteSpace: 'pre-wrap'
};

const chipTrayStyle: CSSProperties = {
  display: 'flex',
  flexWrap: 'wrap',
  gap: '8px',
  maxHeight: '120px',
  overflowY: 'auto'
};

const chipStyle: CSSProperties = {
  borderRadius: '999px',
  padding: '4px 10px',
  fontSize: '0.78rem',
  background: 'rgba(63,115,255,0.12)',
  border: '1px solid rgba(63,115,255,0.5)',
  display: 'flex',
  alignItems: 'center',
  gap: '6px'
};

const chipTypeStyle: CSSProperties = {
  fontSize: '0.65rem',
  letterSpacing: '0.18em',
  textTransform: 'uppercase',
  color: palette.frost
};

const tokenRowStyle: CSSProperties = {
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  padding: '10px 14px',
  borderRadius: '14px',
  background: 'rgba(255,255,255,0.05)',
  fontFamily: "'JetBrains Mono', 'Space Grotesk', monospace",
  fontSize: '0.85rem'
};

const tokenTypeStyle: CSSProperties = {
  textTransform: 'uppercase',
  letterSpacing: '0.08em',
  fontWeight: 600,
  color: palette.frost
};

const tokenValueStyle: CSSProperties = {
  color: palette.muted,
  marginLeft: '18px',
  flex: 1
};

const badgeStyle: CSSProperties = {
  borderRadius: '999px',
  padding: '4px 12px',
  fontSize: '0.75rem',
  textTransform: 'uppercase',
  border: '1px solid rgba(255,255,255,0.2)'
};

const OutputBadge = ({ ok }: { ok: boolean }) => (
  <span
    style={{
      ...badgeStyle,
      color: ok ? '#4de2f6ff' : '#ff7b7b',
      borderColor: ok ? 'rgba(77, 145, 246, 0.4)' : 'rgba(228, 41, 41, 0.4)'
    }}
  >
    {ok ? 'Clean' : 'Issues'}
  </span>
);

const TokenRow = ({ token }: { token: LexerToken }) => (
  <div style={tokenRowStyle}>
    <span style={tokenTypeStyle}>{token.type}</span>
    <span style={tokenValueStyle}>{token.value}</span>
    {(token.line ?? token.column) && (
      <span style={{ color: palette.muted, fontSize: '0.75rem' }}>
        {token.line != null ? `l${token.line}` : ''}
        {token.column != null ? `:${token.column}` : ''}
      </span>
    )}
  </div>
);

const OutputEmpty = ({ message, height }: { message: string; height?: number }) => (
  <div
    style={{
      ...panelChrome,
      minHeight: height ?? 140,
      height: height ?? undefined,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      color: palette.muted,
      textAlign: 'center'
    }}
  >
    {message}
  </div>
);

const LoadingIndicator = ({ height }: { height?: number }) => (
  <div style={{ ...loadingShellStyle, minHeight: height ?? 200 }}>
    <div style={loadingSpinnerStyle} aria-label="Lexing in progress" />
  </div>
);

const OutputErrorPanel = ({
  message,
  diagnostics,
  height
}: {
  message: string;
  diagnostics?: string[];
  height?: number;
}) => (
  <div style={{ ...errorPanelStyle, minHeight: height ?? 160 }}>
    <div style={{ fontWeight: 600, color: '#ff9b9b' }}>Lexer Error</div>
    <div style={{ color: '#ffdede', fontSize: '0.95rem' }}>{message}</div>
    {diagnostics && diagnostics.length ? (
      <div style={{ width: '100%', display: 'flex', flexDirection: 'column', gap: '10px' }}>
        {diagnostics.map((diag, index) => (
          <div key={index} style={diagnosticBlockStyle}>
            {diag}
          </div>
        ))}
      </div>
    ) : null}
  </div>
);

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
    document.body.style.backgroundColor = palette.midnight;
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
      if (!data) {
        const emptyMessage = 'Lexer returned an empty response.';
        setPayload(null);
        setStatusText('Lexing failed');
        setErrorText(emptyMessage);
        setLastRunError(emptyMessage);
        return;
      }

      const rawTokens = (data as { tokens?: LexerToken[] }).tokens;
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
      setErrorText(
        normalizedPayload.ok ? null : normalizedPayload.error ?? 'Unexpected lexer issue.'
      );
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
    <div style={{ ...outputScroll, minHeight: editorHeight, maxHeight: editorHeight }}>
      {tokenList.map((token, index) => (
        <TokenRow token={token} key={`${token.type}-${token.value}-${index}`} />
      ))}
    </div>
  );

  const renderTableTokens = () => (
    <div style={{ ...tableWrapperStyle, minHeight: editorHeight, maxHeight: editorHeight }}>
      <table style={{ width: '100%', borderCollapse: 'separate', borderSpacing: 0 }}>
        <thead>
          <tr>
            <th style={tableHeaderCellStyle}>Lexeme</th>
            <th style={tableHeaderCellStyle}>Token</th>
            <th style={{ ...tableHeaderCellStyle, textAlign: 'right' }}>Line</th>
          </tr>
        </thead>
        <tbody>
          {tokenList.map((token, index) => (
            <tr key={`${token.type}-${token.value}-${index}`}>
              <td style={{ ...tableCellStyle, color: palette.frost }}>{token.value}</td>
              <td style={tableCellStyle}>{token.type}</td>
              <td style={{ ...tableCellStyle, textAlign: 'right', color: palette.muted }}>
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
          ...outputScroll,
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
            color: palette.muted
          }}
        >
          Total Blocks: {blockSummaries.length}
        </div>
        {blockSummaries.map((block) => (
          <div key={block.id} style={blockCardStyle}>
            <div style={blockHeaderStyle}>
              <span>{block.label ?? `Block ${block.id}`}</span>
              <span>
                {block.startLine != null && block.endLine != null
                  ? `Lines ${block.startLine}–${block.endLine}`
                  : 'No line data'}
              </span>
              <span>{block.tokens.length} tokens</span>
            </div>
            <pre style={blockCodeStyle}>{block.snippet.join('\n') || '// empty block'}</pre>
            {block.tokens.length ? (
              <div style={chipTrayStyle}>
                {block.tokens.map((token, index) => (
                  <span key={`${block.id}-${token.type}-${token.value}-${index}`} style={chipStyle}>
                    <span style={chipTypeStyle}>{token.type}</span>
                    <span>{token.value}</span>
                  </span>
                ))}
              </div>
            ) : (
              <div style={{ color: palette.muted, fontSize: '0.78rem' }}>
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
          message={payload.error ?? 'The lexer reported issues. Check your syntax and try again.'}
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
    <div style={appShell}>
      <nav style={navStyles}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '10px', justifySelf: 'start' }}>
        <div style={{ fontSize: '1.4rem', letterSpacing: '0.4em' }}>SPECULA</div>
        </div>
        <div style={navLinksStyle}>
          {navItems.map((item) => (
            <span key={item}>{item}</span>
          ))}
        </div>
        <div style={{ color: palette.muted, fontSize: '0.9rem', justifySelf: 'end', textAlign: 'right' }}>
          v1.0
        </div>
      </nav>

      <section style={heroCopyStyle}>
        <div style={{ color: palette.muted, letterSpacing: '0.45em', fontSize: '0.8rem' }}>
          SPECULA WEB LEXER
        </div>
        <h1 style={heroTitleStyle}>S P E C U L A</h1>
        <p style={heroLeadStyle}>
          Lorem Ipsum Dolor Sit Amet, Consectetur Adipiscing Elit. Sed Do Eiusmod Tempor Incididunt Ut
        </p>
        <div style={ctaRowStyle}>
          <button style={primaryButton} onClick={() => setSource(exampleSnippet)}>
            Load Example
          </button>
          <a
            href="https://github.com/Sky1sBloo/specula-pl"
            target="_blank"
            rel="noreferrer"
            style={{ textDecoration: 'none' }}
          >
            <button style={secondaryButton}>Learn Specula</button>
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
      <section style={studioLayout}>
        <div style={panelChrome}>
          <div style={panelTitleRow}>
            <div style={{ fontWeight: 600 }}>Code Editor</div>
            <div style={tabRowStyle}>
              <button
                style={{
                  ...runButtonStyle,
                  opacity: lexerMutation.isPending ? 0.6 : 1,
                  cursor: lexerMutation.isPending ? 'wait' : 'pointer'
                }}
                onClick={handleRunClick}
                disabled={lexerMutation.isPending}
              >
                Run
              </button>
              <button style={newButtonStyle} onClick={handleNewClick}>
                New
              </button>
            </div>
          </div>
          <div style={{ ...editorShellStyle, height: editorHeight }}>
            <div ref={lineNumberRef} style={{ ...lineNumberColumnStyle, height: editorHeight }}>
              <pre style={lineNumberInnerStyle} aria-hidden="true">
                {lineNumberText}
              </pre>
            </div>
            <textarea
              ref={textAreaRef}
              spellCheck={false}
              style={{ ...editorStyle, height: editorHeight }}
              value={source}
              onScroll={handleEditorScroll}
              onChange={(evt) => setSource(evt.target.value)}
              placeholder="// Start typing Specula code…"
            />
          </div>
          <div style={statusTextStyle}>{statusText}</div>
          {errorText && (
            <div style={{ ...statusTextStyle, color: '#ff7b7b' }}>{errorText}</div>
          )}
        </div>

        <div style={panelChrome}>
          <div style={{ ...panelTitleRow, flexWrap: 'wrap', gap: '12px' }}>
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
              <div style={segmentedRowStyle}>
                {segmentedOptions.map((option) => {
                  const isActive = outputView === option.id;
                  return (
                    <button
                      key={option.id}
                      type="button"
                      style={{
                        ...segmentedButtonBase,
                        background: isActive ? 'rgba(63,115,255,0.4)' : 'transparent',
                        color: isActive ? '#fdfdff' : palette.muted,
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
                  ...buttonBase,
                  padding: '8px 18px',
                  borderRadius: '12px',
                  background: 'rgba(255,255,255,0.08)',
                  color: palette.frost,
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
