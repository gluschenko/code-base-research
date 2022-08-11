import * as React from 'react';
import { Link } from 'react-router-dom';
import styles from './MainLayout.module.scss';

interface IMainLayoutProps {
    children?: React.ReactNode;
}

const MainLayout: React.FC<IMainLayoutProps> = (props) => {
    return (
        <div className={styles.container}>
            <div className={styles.sidebar}>
                <Link to="/">Main</Link>
                <Link to="/settings">Settings</Link>
            </div>
            <div className={styles.content}>
                {props.children}
            </div>
        </div>
    );
};

export default MainLayout;