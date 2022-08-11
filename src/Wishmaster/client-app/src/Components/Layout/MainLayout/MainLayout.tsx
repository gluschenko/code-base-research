import * as React from 'react';
import { Link } from 'react-router-dom';
import styles from './MainLayout.module.scss';
import SidebarLink from './SidebarLink/SidebarLink';

interface IMainLayoutProps {
    children?: React.ReactNode;
}

const MainLayout: React.FC<IMainLayoutProps> = (props) => {
    return (
        <div className={styles.container}>
            <div className={styles.sidebar}>
                <div className={styles.sidebarHeader}>
                    Wishmaster
                </div>
                <SidebarLink url="/" text="Main" icon={["fas", "home"]} />
                <SidebarLink url="/settings" text="Settings" icon={["fas", "cog"]} />
            </div>
            <div className={styles.content}>
                {props.children}
            </div>
        </div>
    );
};

export default MainLayout;