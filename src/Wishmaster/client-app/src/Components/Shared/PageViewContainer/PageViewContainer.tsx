import * as React from 'react';
import styles from './PageViewContainer.module.scss';

interface IPageViewContainerpProps {
    padding?: number;
    margin?: number;
    children?: React.ReactNode;
}

const PageViewContainer: React.FC<IPageViewContainerpProps> = (props) => {
    const { padding = 14, margin = 0 } = props;

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

export default PageViewContainer;