import * as Styles from '../css/Styles';

const OutputBadge = ({ ok }: { ok: boolean }) => (
    <span
        style={{
            ...Styles.badgeStyle,
            color: ok ? '#4de2f6ff' : '#ff7b7b',
            borderColor: ok ? 'rgba(77, 145, 246, 0.4)' : 'rgba(228, 41, 41, 0.4)'
        }}
    >
        {ok ? 'Clean' : 'Issues'}
    </span>
);

export default OutputBadge;
