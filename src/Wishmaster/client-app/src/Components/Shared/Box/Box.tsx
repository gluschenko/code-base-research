import * as React from 'react';
import styles from './Box.module.scss';

interface IBoxProps {
    padding?: number;
    margin?: number;
    children?: React.ReactNode;
}

const Box: React.FC<IBoxProps> = (props) => {
    const { padding = 12, margin = 0 } = props;

    const style: React.CSSProperties = {
        padding: `${padding}px`,
        margin: `${margin}px`,
    };

    return (
        <div className={styles.container} style={style}>
            {props.children}
        </div>
    );
}

export default Box;