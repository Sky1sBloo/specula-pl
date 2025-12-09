import * as Styles from '../css/Styles';

const OutputEmpty = ({ message, height }: { message: string; height?: number }) => (
    <div
        style={{
            ...Styles.panelChrome,
            minHeight: height ?? 140,
            height: height ?? undefined,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            color: Styles.palette.muted,
            textAlign: 'center'
        }}
    >
        {message}
    </div>
);

export default OutputEmpty;
