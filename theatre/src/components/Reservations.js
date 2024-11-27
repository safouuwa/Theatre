import React, { useState, useEffect } from 'react';
import axios from 'axios';

function Reservations() {
    const [reservations, setReservations] = useState([
        { id: 1, showTitle: "The Phantom of the Opera", date: "2024-12-01", customerName: "John Doe" },
        { id: 2, showTitle: "Hamilton", date: "2024-12-05", customerName: "Jane Smith" },
        { id: 3, showTitle: "Les Misérables", date: "2024-12-10", customerName: "Alice Johnson" },
        { id: 4, showTitle: "The Phantom of the Opera", date: "2024-12-02", customerName: "Chris Evans" },
        { id: 5, showTitle: "Hamilton", date: "2024-12-06", customerName: "Emily Clark" }
    ]);
    const [search, setSearch] = useState('');

    const handleSearch = () => {
        if (search.trim() === '') {
            // If search is empty, show all reservations
            setReservations([
                { id: 1, showTitle: "The Phantom of the Opera", date: "2024-12-01", customerName: "John Doe" },
                { id: 2, showTitle: "Hamilton", date: "2024-12-05", customerName: "Jane Smith" },
                { id: 3, showTitle: "Les Misérables", date: "2024-12-10", customerName: "Alice Johnson" },
                { id: 4, showTitle: "The Phantom of the Opera", date: "2024-12-02", customerName: "Chris Evans" },
                { id: 5, showTitle: "Hamilton", date: "2024-12-06", customerName: "Emily Clark" }
            ]);
        } else {
            // Filter mock data based on search query
            setReservations((prev) =>
                prev.filter((res) => res.showTitle.toLowerCase().includes(search.toLowerCase()))
            );
        }
    };
// add this grey code and remove the hardcoded reservations array to fetch data from the API
//
//    useEffect(() => {
//        axios.get('/api/v1/Reservation').then((response) => setReservations(response.data))
//        .catch((error) => { 
//            console.error('Error fetching data: ' + error);
//        });
//    }, []);
//
//    const handleSearch = (e) => {
//        axios.get('/api/v1/admin/reservations?show=${search}').then((response) => setReservations(response.data));
//    };

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