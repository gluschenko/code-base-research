import * as React from 'react';
import AppDataContext from '../../Layout/AppData/AppDataContext/AppDataContext';
import MainLayout from '../../Layout/MainLayout/MainLayout';

const MainPage: React.FC = () => {

    const appDataContext = React.useContext(AppDataContext);

    return (
        <MainLayout>
            <div>
                <div>
                    <pre>
                        {JSON.stringify(appDataContext.data, null, 2)}
                    </pre>
                </div>
            </div>
        </MainLayout>
    );
};

export default MainPage;