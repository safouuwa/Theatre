import React from 'react';
import { Route, Routes } from 'react-router-dom'; 
import Login from './components/Login.js'; // Import the Login component

import Dashboard from './components/Dashboard.js';  // Import Dashboard component
import TheatreShows from './components/TheatreShows.js';  
import Reservations from './components/Reservations.js';

function App() {
  return (
    <div className="App">
      <Routes>  {/* Define Routes for different pages */}
        <Route path="/" element={<Login />} />  {/* Login page route */}
        <Route path="/dashboard" element={<Dashboard />} />  {/* Dashboard page route */}
        <Route path="/theatre-shows" element={<TheatreShows />} />
        <Route path="/reservations" element={<Reservations />} />
      </Routes>
    </div>
  );
}

export default App;