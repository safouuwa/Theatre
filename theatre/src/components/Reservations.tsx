import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import './Reservations.css';

interface Reservation {
  reservationId: number;
  amountOfTickets: number;
  customer?: {
    firstName: string;
    lastName: string;
  };
  theatreShowDate?: {
    theatreShow?: {
      title: string;
    };
    dateAndTime: string;
  };
}

const Reservations: React.FC = () => {
  const navigate = useNavigate();
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [search, setSearch] = useState('');

  const handleReturnDashboard = () => {
    navigate('/dashboard');
  };
 
  useEffect(() => {
    fetchReservations();
  }, []);

  const fetchReservations = (searchTerm = '') => {
    const url = searchTerm 
      ? `http://localhost:5097/api/v1/admin/reservations?show=${searchTerm}`
      : 'http://localhost:5097/api/v1/admin/reservations';

    axios.get<Reservation[]>(url)
      .then((response) => {
        console.log('API Response:', response.data);
        setReservations(response.data);
      })
      .catch((error) => { 
        console.error('Error fetching data:', error);
      });
  };

  const handleSearch = () => {
    fetchReservations(search);
  };        

  return (
    <div className="reservations-container">
      <h1>Reservations</h1>
      <div className="search-container">
        <input 
          value={search} 
          onChange={(e) => setSearch(e.target.value)} 
          placeholder="Search by show title" 
        />
        <button onClick={handleSearch}>Search</button>
        <button onClick={handleReturnDashboard} className="return-home-button">
          Return
        </button>
      </div>
      <table className="reservations-table">
        <thead>
          <tr>
            <th>Reservation ID</th>
            <th>Tickets</th>
            <th>Customer Name</th>
            <th>Show Title</th>
            <th>Date and Time</th>
          </tr>
        </thead>
        <tbody>
          {reservations.length > 0 ? (
            reservations.map((reservation) => (
              <tr key={reservation.reservationId}>
                <td>{reservation.reservationId || 'N/A'}</td>
                <td>{reservation.amountOfTickets || 'N/A'}</td>
                <td>{`${reservation.customer?.firstName || 'N/A'} ${reservation.customer?.lastName || ''}`}</td>
                <td>{reservation.theatreShowDate?.theatreShow?.title || 'N/A'}</td>
                <td>
                  {reservation.theatreShowDate?.dateAndTime
                    ? new Date(reservation.theatreShowDate.dateAndTime).toLocaleString()
                    : 'N/A'}
                </td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan={5}>No reservations found</td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}

export default Reservations;
