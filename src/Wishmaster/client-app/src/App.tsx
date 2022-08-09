import { useState } from 'react'
import logo from './logo.svg'
import { BrowserRouter, Route, Link, Routes } from "react-router-dom";
import MainPage from './Components/Pages/MainPage/MainPage';
import SettingsPage from './Components/Pages/SettingsPage/SettingsPage';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<MainPage />} />
                <Route path="/settings" element={<SettingsPage />} />
                <Route path="*" element={<div>Not found</div>} />
            </Routes>
        </BrowserRouter>
    );
}

export default App
