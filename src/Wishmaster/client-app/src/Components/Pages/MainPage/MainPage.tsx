import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import * as React from 'react';
import AppDataContext from '../../Layout/AppData/AppDataContext/AppDataContext';
import MainLayout from '../../Layout/MainLayout/MainLayout';
import Box from '../../Shared/Box/Box';
import PageViewContainer from '../../Shared/PageViewContainer/PageViewContainer';

const MainPage: React.FC = () => {

    const appDataContext = React.useContext(AppDataContext);

    return (
        <MainLayout>
            <PageViewContainer>
                <Box>
                    <div>
                        <div>
                            <pre>
                                {JSON.stringify(appDataContext.data, null, 2)}
                            </pre>
                        </div>
                        <FontAwesomeIcon icon={["fas", "cog"]} />
                    </div>
                </Box>
                <Box>
                    <div>
                        <div>
                            <pre>
                                {JSON.stringify(appDataContext.data, null, 2)}
                            </pre>
                        </div>
                        <FontAwesomeIcon icon={["fas", "cog"]} />
                    </div>
                </Box>
                <Box>
                    <div>
                        <div>
                            <pre>
                                {JSON.stringify(appDataContext.data, null, 2)}
                            </pre>
                        </div>
                        <FontAwesomeIcon icon={["fas", "cog"]} />
                    </div>
                </Box>
            </PageViewContainer>
        </MainLayout>
    );
};

export default MainPage;