import React from 'react';
import { useNavigate } from 'react-router-dom';
import './Dashboard.css'; // Add styles to make buttons larger and visually appealing

function Dashboard() {
  const navigate = useNavigate();

  // Function to handle button clicks and navigate to the respective pages
  const goToTheatreShows = () => {
    navigate('/theatre-shows');  // Redirect to TheatreShows.js page
  };

  const goToReservations = () => {
    navigate('/reservations');  // Redirect to Reservations.js page
  };

  return (
    <div className="dashboard-container">
      <h2>Admin Dashboard</h2>
      <div className="button-container">
        {/* Button to go to Theatre Shows page */}
        <button onClick={goToTheatreShows} className="dashboard-button">
          Theatre Shows
        </button>

        {/* Button to go to Reservations page */}
        <button onClick={goToReservations} className="dashboard-button">
          Reservations
        </button>
      </div>
    </div>
  );
}

export default Dashboard;
