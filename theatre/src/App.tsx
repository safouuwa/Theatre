import React from 'react';
import { Route, Routes } from 'react-router-dom';
import { AuthProvider } from './components/AuthContext.tsx';
import Login from './components/Login.tsx';
import Dashboard from './components/Dashboard.tsx';
import TheatreShows from './components/TheatreShows.tsx';
import Reservations from './components/Reservations.tsx';
import HomePage from './components/HomePage.tsx';
import ShowDetails from './components/ShowDetails.tsx';

const App: React.FC = () => {
  return (
    <AuthProvider>
      <div className="App">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<Login />} />
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/theatre-shows" element={<TheatreShows />} />
          <Route path="/reservations" element={<Reservations />} />
          <Route path="/show/:id" element={<ShowDetails />} />
        </Routes>
      </div>
    </AuthProvider>
  );
}

export default App;

