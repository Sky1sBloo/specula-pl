import * as Styles from '../css/Styles';

const LoadingIndicator = ({ height }: { height?: number }) => (
    <div style={{
        minHeight: height ?? 200,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center'
    }}>
        <div style={Styles.loadingSpinnerStyle} aria-label="Lexing in progress" />
    </div>
);

export default LoadingIndicator;
