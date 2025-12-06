import { useEffect, useMemo, useRef, useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { tokenizeSource } from './lib/lexer';
import type { CSSProperties } from 'react';
import type { LexerPayload, LexerToken } from './types/token';

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
const tabItems = ['Contract', 'Struct'];

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

const tabChipStyle: CSSProperties = {
  padding: '6px 16px',
  borderRadius: '999px',
  background: 'rgba(255,255,255,0.08)',
  color: palette.frost,
  border: '1px solid rgba(255,255,255,0.08)'
};

const editorStyle: CSSProperties = {
  width: '100%',
  height: '280px',
  background: 'rgba(2,6,26,0.85)',
  borderRadius: '18px',
  border: '1px solid rgba(255,255,255,0.08)',
  padding: '18px',
  color: palette.frost,
  fontSize: '0.95rem',
  fontFamily: "'JetBrains Mono', 'Space Grotesk', monospace",
  resize: 'none',
  outline: 'none',
  lineHeight: 1.6
};

const statusTextStyle: CSSProperties = {
  fontSize: '0.85rem',
  color: palette.muted,
  marginTop: '12px'
};

const outputScroll: CSSProperties = {
  maxHeight: '280px',
  overflowY: 'auto',
  display: 'flex',
  flexDirection: 'column',
  gap: '10px'
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
      color: ok ? '#4df6b6' : '#ff7b7b',
      borderColor: ok ? 'rgba(77,246,182,0.4)' : 'rgba(255,123,123,0.4)'
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

const OutputEmpty = ({ message }: { message: string }) => (
  <div
    style={{
      ...panelChrome,
      minHeight: '140px',
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

export default function App() {
  const [source, setSource] = useState(exampleSnippet);
  const [payload, setPayload] = useState<LexerPayload | null>(null);
  const [statusText, setStatusText] = useState('Lexing preview example…');
  const [errorText, setErrorText] = useState<string | null>(null);
  const debounceRef = useRef<number>();

  useEffect(() => {
    document.body.style.margin = '0';
    document.body.style.backgroundColor = palette.midnight;
    document.body.style.fontFamily = "'Space Grotesk', 'Segoe UI', sans-serif";
    document.body.style.minHeight = '100vh';
    return () => {
      document.body.removeAttribute('style');
    };
  }, []);

  const lexerMutation = useMutation({
    mutationFn: tokenizeSource,
    onSuccess: (data) => {
      setPayload(data);
      const descriptor = data.tokens.length === 1 ? 'token' : 'tokens';
      setStatusText(
        data.ok
          ? `Lexing complete • ${data.tokens.length} ${descriptor}`
          : 'Lexer reported diagnostics'
      );
      setErrorText(data.ok ? null : data.error ?? 'Unexpected lexer issue.');
    },
    onError: (err: unknown) => {
      const message = err instanceof Error ? err.message : 'Unknown lexer failure.';
      setErrorText(message);
      setStatusText('Lexing failed');
    }
  });

  useEffect(() => {
    window.clearTimeout(debounceRef.current);
    if (!source.trim()) {
      setPayload(null);
      setStatusText('Waiting for Specula input…');
      setErrorText(null);
      return;
    }

    debounceRef.current = window.setTimeout(() => {
      setStatusText('Lexing Specula source…');
      setErrorText(null);
      lexerMutation.mutate(source);
    }, 520);

    return () => {
      window.clearTimeout(debounceRef.current);
    };
  }, [source]);

  const tokenList = useMemo(() => payload?.tokens ?? [], [payload]);

  const renderOutput = () => {
    if (lexerMutation.isPending) {
      return <OutputEmpty message="Crunching characters through the DFA…" />;
    }

    if (!payload) {
      return <OutputEmpty message="// Lexical analysis output will appear here" />;
    }

    if (!payload.ok) {
      return (
        <OutputEmpty
          message={payload.error ?? 'The lexer reported issues. Check your syntax and try again.'}
        />
      );
    }

    if (!tokenList.length) {
      return <OutputEmpty message="No tokens produced yet." />;
    }

    return (
      <div style={outputScroll}>
        {tokenList.map((token, index) => (
          <TokenRow token={token} key={`${token.type}-${token.value}-${index}`} />
        ))}
      </div>
    );
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

      <section style={studioLayout}>
        <div style={panelChrome}>
          <div style={panelTitleRow}>
            <div style={{ fontWeight: 600 }}>Code Editor</div>
            <div style={tabRowStyle}>
              {tabItems.map((tab) => (
                <span key={tab} style={tabChipStyle}>
                  {tab}
                </span>
              ))}
            </div>
          </div>
          <textarea
            spellCheck={false}
            style={editorStyle}
            value={source}
            onChange={(evt) => setSource(evt.target.value)}
            placeholder="// Start typing Specula code…"
          />
          <div style={statusTextStyle}>{statusText}</div>
          {errorText && (
            <div style={{ ...statusTextStyle, color: '#ff7b7b' }}>{errorText}</div>
          )}
        </div>

        <div style={panelChrome}>
          <div style={panelTitleRow}>
            <div style={{ fontWeight: 600, display: 'flex', alignItems: 'center', gap: '8px' }}>
              Output <OutputBadge ok={payload ? !!payload.ok : true} />
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
          {renderOutput()}
        </div>
      </section>
    </div>
  );
}
