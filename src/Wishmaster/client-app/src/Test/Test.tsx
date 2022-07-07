import * as React from 'react';
import styles from './Test.module.scss';

const Test: React.FC = () => {
    return (
        <div className={styles.main}>
            <div className={styles.text}>
                1234
            </div>
        </div>
    );
};

export default Test;