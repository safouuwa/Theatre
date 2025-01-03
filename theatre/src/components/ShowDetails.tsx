import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';
import ReservationForm from './ReservationForm.tsx';
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
        theatreShowDateId: number;
        dateAndTime: string;
    }[];
}

const ShowDetails: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [show, setShow] = useState<TheatreShow | null>(null);
    const [showReservationForm, setShowReservationForm] = useState(false);

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
            {!showReservationForm ? (
                <>
                    <h1>{show.title}</h1>
                    <p className="description">{show.description}</p>
                    <div className="details">
                        <p><strong>Price:</strong> ${show.price}</p>
                        <p><strong>Venue:</strong> {show.venue?.name}</p>
                        <p><strong>Date and Time:</strong> {new Date(show.theatreShowDates[0].dateAndTime).toLocaleString()}</p>
                    </div>
                    <button 
                        className="reserve-button"
                        onClick={() => setShowReservationForm(true)}
                    >
                        Reserve Tickets
                    </button>
                </>
            ) : (
                <ReservationForm
                    showId={show.theatreShowId}
                    theatreShowDateId={show.theatreShowDates[0].theatreShowDateId}
                    showTitle={show.title}
                    showDate={show.theatreShowDates[0].dateAndTime}
                    price={show.price}
                    onCancel={() => setShowReservationForm(false)}
                />
            )}
        </div>
    );
};

export default ShowDetails;

