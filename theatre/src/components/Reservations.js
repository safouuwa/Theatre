import React, { useState, useEffect } from 'react';
import axios from 'axios';

function Reservations() {
    const [reservations, setReservations] = useState([]);
    const [search, setSearch] = useState('');

    useEffect(() => {
        axios.get('/api/v1/Reservation').then((response) => setReservations(response.data));
    }, []);

    const handleSearch = (e) => {
        axios.get('/api/v1/admin/reservations?show=${search}').then((response) => setReservations(response.data));
    };

    return (
        <div>
            <h1>Reservations</h1>
            <input value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Search by show title" />
            <button onClick={handleSearch}>Search</button>
            <ul>
                {reservations.map((res) => (
                    <li key={res.id}>
                        {res.showTitle} - {res.date} - {res.customerName}
                </li>
                ))}
            </ul>
        </div>
    );
}

export default Reservations;