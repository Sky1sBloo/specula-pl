import * as Styles from '../css/Styles';

const OutputErrorPanel = ({
    message,
    diagnostics,
    height
}: {
    message: string;
    diagnostics?: string[];
    height?: number;
}) => (
    <div style={{
        width: '100%',
        height: height ?? 160,
        background: 'rgba(2,6,26,0.85)',
        borderRadius: '12px',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        gap: '12px',
        textAlign: 'center'
    }}>
        <div style={{
            fontSize: '2rem',
            fontFamily: "'JetBrains Mono', 'Space Grotesk', monospace",
            color: '#ff9b9b'
        }}>):</div>
        <div style={{ fontWeight: 600, color: '#ff9b9b', fontSize: '1.1rem' }}>Lexer Error</div>
    </div>
);

export default OutputErrorPanel;
