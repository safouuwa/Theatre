import React from 'react';
import { Route, Routes } from 'react-router-dom'; 
import Login from './components/Login.js'; // Import the Login component

import Dashboard from './components/Dashboard.js';  // Import Dashboard component

function App() {
  return (
    <div className="App">
      <Routes>  {/* Define Routes for different pages */}
        <Route path="/" element={<Login />} />  {/* Login page route */}
        <Route path="/dashboard" element={<Dashboard />} />  {/* Dashboard page route */}
      </Routes>
    </div>
  );
}

export default App;