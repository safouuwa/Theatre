import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './AuthContext.tsx'
import './Dashboard.css';

const Dashboard: React.FC = () => {
  const { isAuthenticated, isAdmin, logout, customerData } = useAuth();
  const handleLogoutClick = async () => {
    await logout();
    navigate('/');
  };
  const navigate = useNavigate();

  const goToTheatreShows = () => {
    navigate('/theatre-shows');
  };

  const goToReservations = () => {
    navigate('/reservations');
  };

  return (
    <div className="dashboard-container">
      <h2>Admin Dashboard</h2>
      <button className="logout-button" onClick={handleLogoutClick}>Logout</button>
      <div className="button-container">
        <button onClick={goToTheatreShows} className="dashboard-button1">
          Theatre Shows
        </button>
        <button onClick={goToReservations} className="dashboard-button2">
          Reservations
        </button>
      </div>
    </div>
  );
}

export default Dashboard;
