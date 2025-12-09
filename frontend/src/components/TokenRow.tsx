import * as Styles from '../css/Styles';
import type { LexerToken } from '../types/token';

const TokenRow = ({ token }: { token: LexerToken }) => (
    <div style={Styles.tokenRowStyle}>
        <span style={Styles.tokenTypeStyle}>{token.type}</span>
        <span style={Styles.tokenValueStyle}>{token.value}</span>
        {(token.line ?? token.column) && (
            <span style={{ color: Styles.palette.muted, fontSize: '0.75rem' }}>
                {token.line != null ? `l${token.line}` : ''}
                {token.column != null ? `:${token.column}` : ''}
            </span>
        )}
    </div>
);

export default TokenRow;
