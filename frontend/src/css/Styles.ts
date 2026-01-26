import { CSSProperties } from "react";

export const palette = {
    midnight: '#01010f',
    ocean: '#041137',
    royal: '#1438ff',
    electric: '#3f73ff',
    frost: 'rgba(255, 255, 255, 0.72)',
    muted: 'rgba(255, 255, 255, 0.45)',
    glass: 'rgba(7, 12, 40, 0.76)'
};

export const appShell: CSSProperties = {
    minHeight: '100vh',
    padding: '32px 40px 48px',
    backgroundColor: palette.midnight,
    backgroundImage:
        'radial-gradient(circle at 20% 20%, rgba(31,78,255,0.25), transparent 55%), radial-gradient(circle at 80% 0%, rgba(14,54,166,0.32), transparent 45%), repeating-linear-gradient(-80deg, rgba(4,6,30,0.95) 0px, rgba(4,6,30,0.95) 60px, rgba(7, 24, 65, 0.7) 120px, rgba(5, 16, 51, 0.7) 180px)',
    color: '#f8fbff'
};

export const navStyles: CSSProperties = {
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

export const navLinksStyle: CSSProperties = {
    display: 'flex',
    gap: '32px',
    color: palette.muted,
    textTransform: 'uppercase',
    fontSize: '0.85rem',
    letterSpacing: '0.2em',
    justifySelf: 'center'
};

export const heroCopyStyle: CSSProperties = {
    textAlign: 'center',
    maxWidth: '640px',
    margin: '0 auto 40px'
};

export const heroTitleStyle: CSSProperties = {
    fontSize: '3.3rem',
    letterSpacing: '0.08em',
    marginBottom: '16px'
};

export const heroLeadStyle: CSSProperties = {
    color: palette.frost,
    lineHeight: 1.6,
    fontSize: '1.05rem'
};

export const ctaRowStyle: CSSProperties = {
    display: 'flex',
    justifyContent: 'center',
    gap: '16px',
    marginTop: '28px'
};

export const buttonBase: CSSProperties = {
    border: 'none',
    borderRadius: '999px',
    padding: '14px 28px',
    fontSize: '0.95rem',
    cursor: 'pointer',
    fontWeight: 600,
    transition: 'transform 220ms ease, box-shadow 220ms ease'
};

export const primaryButton: CSSProperties = {
    ...buttonBase,
    backgroundColor: 'rgba(255, 255, 255, 1)',
    color: '#000000ff',
    boxShadow: '0 10px 30px rgba(60, 44, 199, 0.35)'
};

export const secondaryButton: CSSProperties = {
    ...buttonBase,
    backgroundColor: 'rgba(255, 255, 255, 0.1)',
    color: '#fffefeff',
    border: '3px solid rgba(255,255,255,0.18)'
};

export const studioLayout: CSSProperties = {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))',
    gap: '24px'
};

export const panelChrome: CSSProperties = {
    background: palette.glass,
    borderRadius: '24px',
    border: '1px solid rgba(255,255,255,0.08)',
    padding: '22px',
    minHeight: '360px',
    boxShadow: '0 20px 60px rgba(2,0,20,0.55)',
    backdropFilter: 'blur(16px)'
};

export const panelTitleRow: CSSProperties = {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '16px'
};

export const tabRowStyle: CSSProperties = {
    display: 'flex',
    gap: '8px',
    fontSize: '0.85rem'
};

export const editorActionButton: CSSProperties = {
    ...buttonBase,
    padding: '8px 18px',
    borderRadius: '12px',
    background: 'rgba(255,255,255,0.08)',
    color: palette.frost,
    border: '1px solid rgba(255,255,255,0.12)'
};

export const runButtonStyle: CSSProperties = {
    ...editorActionButton,
    background: 'rgba(63,115,255,0.2)',
    borderColor: 'rgba(63,115,255,0.6)'
};

export const newButtonStyle: CSSProperties = {
    ...editorActionButton,
    background: 'rgba(255,255,255,0.04)',
    borderColor: 'rgba(255,255,255,0.18)'
};

export const editorShellStyle: CSSProperties = {
    display: 'grid',
    gridTemplateColumns: 'auto 1fr',
    background: 'rgba(2,6,26,0.85)',
    borderRadius: '18px',
    border: '1px solid rgba(255,255,255,0.08)',
    overflow: 'hidden'
};

export const lineNumberColumnStyle: CSSProperties = {
    display: 'flex',
    justifyContent: 'flex-end',
    alignItems: 'flex-start',
    background: 'rgba(1,3,20,0.85)',
    color: palette.muted,
    fontFamily: "'JetBrains Mono', 'Space Grotesk', monospace",
    fontSize: '0.95rem',
    padding: '18px 12px',
    textAlign: 'right',
    borderRight: '1px solid rgba(255,255,255,0.08)',
    minWidth: '52px',
    boxSizing: 'border-box',
    overflow: 'hidden',
    userSelect: 'none'
};

export const lineNumberInnerStyle: CSSProperties = {
    margin: 0,
    lineHeight: 1.6,
    whiteSpace: 'pre'
};

export const editorStyle: CSSProperties = {
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

export const statusTextStyle: CSSProperties = {
    fontSize: '0.85rem',
    color: palette.muted,
    marginTop: '12px'
};

export const outputScroll: CSSProperties = {
    overflowY: 'auto',
    display: 'flex',
    flexDirection: 'column',
    gap: '10px'
};

export const loadingShellStyle: CSSProperties = {
    ...panelChrome,
    minHeight: 200,
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center'
};

export const loadingSpinnerStyle: CSSProperties = {
    width: '54px',
    height: '54px',
    borderRadius: '50%',
    border: '3px solid rgba(255,255,255,0.45)',
    borderTopColor: 'transparent',
    animation: 'spin 1s linear infinite'
};

export const errorPanelStyle: CSSProperties = {
    ...panelChrome,
    display: 'flex',
    flexDirection: 'column',
    gap: '12px',
    alignItems: 'flex-start',
    borderColor: 'rgba(255,123,123,0.3)'
};

export const diagnosticBlockStyle: CSSProperties = {
    width: '100%',
    background: 'rgba(255,123,123,0.08)',
    borderRadius: '12px',
    padding: '12px 14px',
    fontSize: '0.85rem',
    color: palette.frost,
    fontFamily: "'JetBrains Mono', 'Space Grotesk', monospace"
};

export const segmentedRowStyle: CSSProperties = {
    display: 'flex',
    background: 'rgba(255,255,255,0.06)',
    borderRadius: '14px',
    padding: '4px',
    minWidth: '200px'
};

export const segmentedRowStyle4: CSSProperties = {
    display: 'grid',
    gridTemplateColumns: 'repeat(4, minmax(0, 1fr))',
    background: 'rgba(255,255,255,0.06)',
    borderRadius: '14px',
    padding: '4px',
    minWidth: '280px'
};

export const segmentedButtonBase: CSSProperties = {
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

export const tableWrapperStyle: CSSProperties = {
    overflowY: 'auto',
    overflowX: 'auto',
    borderRadius: '16px',
    border: '1px solid rgba(255,255,255,0.06)'
};

export const tableHeaderCellStyle: CSSProperties = {
    textAlign: 'left',
    padding: '8px 12px',
    fontSize: '0.75rem',
    textTransform: 'uppercase',
    letterSpacing: '0.18em',
    color: palette.muted,
    borderBottom: '1px solid rgba(255,255,255,0.12)'
};

export const tableCellStyle: CSSProperties = {
    padding: '10px 12px',
    fontSize: '0.85rem',
    borderBottom: '1px solid rgba(255,255,255,0.06)'
};

export const blockCardStyle: CSSProperties = {
    padding: '14px',
    borderRadius: '16px',
    background: 'rgba(255,255,255,0.04)',
    border: '1px solid rgba(255,255,255,0.08)',
    display: 'flex',
    flexDirection: 'column',
    gap: '10px'
};

export const blockHeaderStyle: CSSProperties = {
    display: 'flex',
    justifyContent: 'space-between',
    flexWrap: 'wrap',
    gap: '6px',
    fontSize: '0.8rem',
    textTransform: 'uppercase',
    letterSpacing: '0.18em',
    color: palette.muted
};

export const blockCodeStyle: CSSProperties = {
    margin: 0,
    borderRadius: '12px',
    background: 'rgba(1,1,15,0.8)',
    padding: '10px 12px',
    fontFamily: "'JetBrains Mono', 'Space Grotesk', monospace",
    fontSize: '0.85rem',
    whiteSpace: 'pre-wrap'
};

export const chipTrayStyle: CSSProperties = {
    display: 'flex',
    flexWrap: 'wrap',
    gap: '8px',
    maxHeight: '120px',
    overflowY: 'auto'
};

export const chipStyle: CSSProperties = {
    borderRadius: '999px',
    padding: '4px 10px',
    fontSize: '0.78rem',
    background: 'rgba(63,115,255,0.12)',
    border: '1px solid rgba(63,115,255,0.5)',
    display: 'flex',
    alignItems: 'center',
    gap: '6px'
};

export const chipTypeStyle: CSSProperties = {
    fontSize: '0.65rem',
    letterSpacing: '0.18em',
    textTransform: 'uppercase',
    color: palette.frost
};

export const tokenRowStyle: CSSProperties = {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: '10px 14px',
    borderRadius: '14px',
    background: 'rgba(255,255,255,0.05)',
    fontFamily: "'JetBrains Mono', 'Space Grotesk', monospace",
    fontSize: '0.85rem'
};

export const tokenTypeStyle: CSSProperties = {
    textTransform: 'uppercase',
    letterSpacing: '0.08em',
    fontWeight: 600,
    color: palette.frost
};

export const tokenValueStyle: CSSProperties = {
    color: palette.muted,
    marginLeft: '18px',
    flex: 1
};

export const badgeStyle: CSSProperties = {
    borderRadius: '999px',
    padding: '4px 12px',
    fontSize: '0.75rem',
    textTransform: 'uppercase',
    border: '1px solid rgba(255,255,255,0.2)'
};
