import React from 'react';
import { useNavigate } from 'react-router-dom';
import './Dashboard.css';

function Dashboard() {
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
      <div className="button-container">
        {/* Button to go to Theatre Shows page */}
        <button onClick={goToTheatreShows} className="dashboard-button1">
          Theatre Shows
        </button>

        {/* Button to go to Reservations page */}
        <button onClick={goToReservations} className="dashboard-button2">
          Reservations
        </button>
      </div>
    </div>
  );
}

export default Dashboard;
