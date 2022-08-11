import * as React from 'react';
import { AppDataClient, AppDataViewModel } from '../../../models';
import AppDataContext from './AppDataContext/AppDataContext';

interface IAppDataProps {
    children?: React.ReactNode;
}

const AppData: React.FC<IAppDataProps> = (props) => {

    const [isReady, toggleReady] = React.useState<boolean>(false);
    const [appData, setAppData] = React.useState<AppDataViewModel | null>(null);

    React.useEffect(() => {
        const appDataClient = new AppDataClient();

        appDataClient.getAppData()
            .then(x => setAppData(x))
            // TODO
            .catch(x => console.log(x));

        toggleReady(true);
    }, []);

    return (
        <AppDataContext.Provider value={{ data: appData }}>
            {isReady ? <>{props.children}</> : <div>Loading...</div>}
        </AppDataContext.Provider>
    );
};

export default AppData;