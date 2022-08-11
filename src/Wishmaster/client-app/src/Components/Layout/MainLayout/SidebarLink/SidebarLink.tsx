import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import classNames from 'classnames';
import * as React from 'react';
import { NavLink } from 'react-router-dom';
import styles from './SidebarLink.module.scss';

interface ISidebarLinkProps {
    text: string;
    icon: IconProp;
    url: string;
}

const SidebarLink: React.FC<ISidebarLinkProps> = (props) => {
    return (
        <NavLink to={props.url} className={(props) => classNames(styles.container, { [styles.active]: props.isActive })}>
            <FontAwesomeIcon icon={props.icon} className={styles.icon} />
            <span>{props.text}</span>
        </NavLink>
    );
};

export default SidebarLink;