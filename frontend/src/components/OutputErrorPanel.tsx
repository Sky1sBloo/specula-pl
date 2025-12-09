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
    <div style={{ ...Styles.errorPanelStyle, minHeight: height ?? 160 }}>
        <div style={{ fontWeight: 600, color: '#ff9b9b' }}>Lexer Error</div>
        <div style={{ color: '#ffdede', fontSize: '0.95rem' }}>{message}</div>
        {diagnostics && diagnostics.length ? (
            <div style={{ width: '100%', display: 'flex', flexDirection: 'column', gap: '10px' }}>
                {diagnostics.map((diag, index) => (
                    <div key={index} style={Styles.diagnosticBlockStyle}>
                        {diag}
                    </div>
                ))}
            </div>
        ) : null}
    </div>
);

export default OutputErrorPanel;
