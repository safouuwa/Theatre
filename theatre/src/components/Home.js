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
                console.log(response.data); // Inspect the data
                const futureShows = response.data.filter(show => new Date(show.dateAndTime) > new Date());
                console.log(futureShows); // Inspect the filtered shows
                setShows(futureShows);
            })
            .catch(error => console.error('Error fetching shows:', error));
    }, []);

    useEffect(() => {
        console.log(shows); // Inspect the shows state
    }, [shows]);

    const handleShowClick = (showId) => {
        navigate(`/shows/${showId}`);
    };

    return (
        <div className="home-container">
            <h1>Available Shows</h1>
            <ul className="shows-list">
                {shows.length > 0 ? (
                    shows.map(show => (
                        <li key={show.id} className="show-item" onClick={() => handleShowClick(show.id)}>
                            <h2>{show.name}</h2>
                            <p>{show.details}</p>
                            <p>Price: ${show.cost}</p>
                        </li>
                    ))
                ) : (
                    <p>No shows available</p>
                )}
            </ul>
        </div>
    );
}

export default Home;