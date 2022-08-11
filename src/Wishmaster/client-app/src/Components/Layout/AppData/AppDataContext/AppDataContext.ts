import * as React from 'react';
import { AppDataViewModel } from '../../../../models';

interface IAppDataContextProps {
    data: AppDataViewModel | null;
}

const AppDataContext = React.createContext<IAppDataContextProps>({
    data: null,
});

export default AppDataContext;