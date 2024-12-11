import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';
import './ShowDetails.css';

function ShowDetails() {
    const { id } = useParams();
    const [show, setShow] = useState(null);

    useEffect(() => {
        axios.get(`http://localhost:5097/api/v1/TheatreShow/${id}`)
            .then(response => {
                setShow(response.data);
            })
            .catch(error => console.error('Error fetching show details:', error));
    }, [id]);

    if (!show) {
        return <div>Loading...</div>;
    }

    return (
        <div className="show-details-container">
            <h1>{show.title}</h1>
            <p>{show.description}</p>
            <p>Date and Time: {new Date(show.dateAndTime).toLocaleString()}</p>
            <p>Price: ${show.price}</p>
            <p>Venue: {show.venue.name}</p>
            <p>Address: {show.venue.address}</p>
        </div>
    );
}

export default ShowDetails;