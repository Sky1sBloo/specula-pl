import * as Styles from '../css/Styles';

const LoadingIndicator = ({ height }: { height?: number }) => (
    <div style={{ ...Styles.loadingShellStyle, minHeight: height ?? 200 }}>
        <div style={Styles.loadingSpinnerStyle} aria-label="Lexing in progress" />
    </div>
);

export default LoadingIndicator;
