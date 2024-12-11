import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import './Home.css';

function Home() {
    const [shows, setShows] = useState([]);
    const navigate = useNavigate();

    useEffect(() => {
        axios.get('http://localhost:5097/api/v1/TheatreShow')
            .then(response => {
                const futureShows = response.data.filter(show => new Date(show.dateAndTime) > new Date());
                setShows(futureShows);
            })
            .catch(error => console.error('Error fetching shows:', error));
    }, []);

    const handleShowClick = (showId) => {
        navigate(`/shows/${showId}`);
    };

    return (
        <div className="home-container">
            <h1>Available Shows</h1>
            <ul className="shows-list">
                {shows.map(show => (
                    <li key={show.theatreShowId} className="show-item" onClick={() => handleShowClick(show.theatreShowId)}>
                        <h2>{show.title}</h2>
                        <p>{show.description}</p>
                        <p>Price: ${show.price}</p>
                        <p>Venue: {show.venue.name}</p>
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default Home;