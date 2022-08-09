import * as React from 'react';
import { Link } from 'react-router-dom';

interface IMainLayoutProps {
    children?: React.ReactNode;
}

const MainLayout: React.FC<IMainLayoutProps> = (props) => {
    return (
        <div>
            <div>
                <Link to="/">Main</Link>
                <Link to="/settings">Settings</Link>
            </div>
            <div>{props.children}</div>
        </div>
    );
};

export default MainLayout;