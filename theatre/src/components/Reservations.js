import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './Reservations.css';


function Reservations() {
    const [reservations, setReservations] = useState([]);
    const [search, setSearch] = useState('');
 
    useEffect(() => {
        axios.get('http://localhost:5097/api/v1/admin/reservations').then((response) => setReservations(response.data))
        .catch((error) => { 
            console.error('Error fetching data: ' + error);
        });
    }, []);

    const handleSearch = (e) => {
        axios.get('http://localhost:5097/api/v1/admin/reservations?show=e').then((response) => setReservations(response.data));
    };        

    return (
        <div>
            <h1>Reservations</h1>
            <input value={search} 
            onChange={(e) => setSearch(e.target.value)} 
            placeholder="Search by show title" />
            <button onClick={handleSearch}>Search</button>
            <ul>
                {reservations.map((reservation) => (
                    <li key={reservation.id}>
                        {reservation.showTitle} - {reservation.date} - {reservation.customerName}
                </li>
                ))}
            </ul>
        </div>
    );
}

export default Reservations;