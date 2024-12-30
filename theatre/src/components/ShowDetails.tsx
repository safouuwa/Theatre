import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';
import './ShowDetails.css';

interface TheatreShow {
    theatreShowId: number;
    title: string;
    description: string;
    price: number;
    venue: {
        name: string;
    };
    theatreShowDates: {
        dateAndTime: string;
    }[];
}

const ShowDetails: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [show, setShow] = useState<TheatreShow | null>(null);

    useEffect(() => {
        axios.get<TheatreShow>(`http://localhost:5097/api/v1/TheatreShow/id/${id}`)
            .then(response => {
                setShow(response.data);
            })
            .catch(error => {
                console.error('Error fetching show details:', error);
            });
    }, [id]);

    if (!show) {
        return <div>Loading...</div>;
    }

    return (
        <div className="show-details-container">
            <h1>{show.title}</h1>
            <p>{show.description}</p>
            <p>Price: ${show.price}</p>
            <p>Venue: {show.venue?.name}</p>
            <p>Date and Time: {new Date(show.theatreShowDates[0].dateAndTime).toLocaleString()}</p>
        </div>
    );
};

export default ShowDetails;