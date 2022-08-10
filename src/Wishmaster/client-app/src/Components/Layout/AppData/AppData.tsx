import * as React from 'react';
import { AppDataClient, AppDataViewModel } from '../../../models';

interface IAppDataProps {
    children?: React.ReactNode;
}

const AppData: React.FC<IAppDataProps> = (props) => {

    const [appData, setAppData] = React.useState<AppDataViewModel | null>(null);

    React.useEffect(() => {
        const appDataClient = new AppDataClient();

        appDataClient.getAppData()
            .then(x => setAppData(x))
            // TODO
            .catch(x => console.log(x));
    }, []);

    return (
        <div>
            <div>
                <pre>
                    {JSON.stringify(appData, null, 2)}
                </pre>
            </div>
            {props.children}
        </div>
    );
};

export default AppData;