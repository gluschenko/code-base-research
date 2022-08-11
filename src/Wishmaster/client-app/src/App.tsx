import './App.scss';
import { BrowserRouter, Route, Link, Routes } from "react-router-dom";
import MainPage from './Components/Pages/MainPage/MainPage';
import SettingsPage from './Components/Pages/SettingsPage/SettingsPage';
import AppData from './Components/Layout/AppData/AppData';
import { library } from '@fortawesome/fontawesome-svg-core';
import { fas } from '@fortawesome/free-solid-svg-icons';
import { far } from '@fortawesome/free-regular-svg-icons';

library.add(fas, far);

function App() {
    return (
        <AppData>
            <BrowserRouter>
                <Routes>
                    <Route path="/" element={<MainPage />} />
                    <Route path="/settings" element={<SettingsPage />} />
                    <Route path="*" element={<div>Not found</div>} />
                </Routes>
            </BrowserRouter>
        </AppData>
    );
}

export default App;
