import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './AuthContext.tsx';
import './HomePage.css';

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

const HomePage: React.FC = () => {
    const [shows, setShows] = useState<TheatreShow[]>([]);
    const navigate = useNavigate();
    const { isAuthenticated, isAdmin, logout, customerData } = useAuth();

    useEffect(() => {
        if (isAdmin) {
            navigate('/dashboard');
        }
    }, [isAdmin, navigate]);

    useEffect(() => {
        axios.get<TheatreShow[]>('http://localhost:5097/api/v1/TheatreShow')
            .then(response => {
                const futureShows = response.data.filter(show => 
                    new Date(show.theatreShowDates[0].dateAndTime) > new Date()
                );
                setShows(futureShows);
            })
            .catch(error => {
                console.error('Error fetching shows:', error);
            });
    }, []);

    const handleShowClick = (id: number) => {
        navigate(`/show/${id}`);
    };
    
    const handleLoginClick = () => {
        navigate('/login');
    };

    const handleLogoutClick = async () => {
        await logout();
        navigate('/');
    };

    return (
        <div>
            {isAuthenticated ? (
                <div>
                    <p>Welcome, {customerData?.firstName || 'User'}!</p>
                    <button className="logout-button" onClick={handleLogoutClick}>Logout</button>
                </div>
            ) : (
                <button className="login-button" onClick={handleLoginClick}>Login</button>
            )}
            <div className="homepage-container">
                <h1>Available Shows</h1>
                <ul>
                    {shows.map(show => (
                        <li key={show.theatreShowId} onClick={() => handleShowClick(show.theatreShowId)}>
                            {show.title} - {new Date(show.theatreShowDates[0].dateAndTime).toLocaleString()}
                        </li>
                    ))}
                </ul>
            </div>
        </div>
    );
};

export default HomePage;

