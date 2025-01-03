import React from 'react';
import { Route, Routes } from 'react-router-dom';
import { AuthProvider } from './components/AuthContext.tsx';
import { ShoppingCartProvider } from './components/ShoppingCartContext.tsx';
import Login from './components/Login.tsx';
import Dashboard from './components/Dashboard.tsx';
import TheatreShows from './components/TheatreShows.tsx';
import Reservations from './components/Reservations.tsx';
import HomePage from './components/HomePage.tsx';
import ShowDetails from './components/ShowDetails.tsx';
import ShoppingCart from './components/ShoppingCart.tsx';

const App: React.FC = () => {
  return (
    <AuthProvider>
      <ShoppingCartProvider>
        <div className="App">
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/login" element={<Login />} />
            <Route path="/dashboard" element={<Dashboard />} />
            <Route path="/theatre-shows" element={<TheatreShows />} />
            <Route path="/reservations" element={<Reservations />} />
            <Route path="/show/:id" element={<ShowDetails />} />
            <Route path="/cart" element={<ShoppingCart />} />
          </Routes>
        </div>
      </ShoppingCartProvider>
    </AuthProvider>
  );
}

export default App;

