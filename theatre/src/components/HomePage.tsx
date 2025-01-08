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

interface Venue {
    venueId: number;
    name: string;
}

const HomePage: React.FC = () => {
    const [shows, setShows] = useState<TheatreShow[]>([]);
    const [venues, setVenues] = useState<Venue[]>([]);
    const [filters, setFilters] = useState({
        title: '',
        description: '',
        venueId: '',
        month: '',
        sortBy: 'title',
        sortOrder: 'asc'
    });
    const navigate = useNavigate();
    const { isAuthenticated, isAdmin, logout, customerData } = useAuth();

    useEffect(() => {
        if (isAdmin) {
            navigate('/dashboard');
        }
    }, [isAdmin, navigate]);

    useEffect(() => {
        fetchVenues();
        fetchShows();
    }, [filters]);

    const fetchVenues = () => {
        axios.get<Venue[]>('http://localhost:5097/api/v1/Venue')
            .then(response => {
                setVenues(response.data);
            })
            .catch(error => {
                console.error('Error fetching venues:', error);
            });
    };

    const fetchShows = () => {
        const { title, description, venueId, month, sortBy, sortOrder } = filters;
        let url = `http://localhost:5097/api/v1/TheatreShow/filter/${sortBy}/${sortOrder}?title=${title}&description=${description}`;
        if (venueId) {
            url += `&location=${venueId}`;
        }
        if (month) {
            const startDate = new Date(month);
            const endDate = new Date(startDate.getFullYear(), startDate.getMonth() + 1, 0);
            url += `&startDate=${startDate.toISOString()}&endDate=${endDate.toISOString()}`;
        }
        axios.get<TheatreShow[]>(url)
            .then(response => {
                setShows(response.data);
            })
            .catch(error => {
                console.error('Error fetching shows:', error);
            });
    };

    const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setFilters(prevFilters => ({
            ...prevFilters,
            [name]: value
        }));
    };

    const handleShowClick = (theatreShowId: number) => {
        navigate(`/show/${theatreShowId}`);
    };

    const handleLoginClick = () => {
        navigate('/login');
    };

    const handleLogoutClick = () => {
        logout();
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
                <div className="filter-container">
                    <input
                        type="text"
                        name="title"
                        placeholder="Search by title"
                        value={filters.title}
                        onChange={handleFilterChange}
                    />
                    <input
                        type="text"
                        name="description"
                        placeholder="Search by description"
                        value={filters.description}
                        onChange={handleFilterChange}
                    />
                    <select name="venueId" value={filters.venueId} onChange={handleFilterChange}>
                        <option value="">Select Venue</option>
                        {venues.map(venue => (
                            <option key={venue.venueId} value={venue.venueId}>{venue.name}</option>
                        ))}
                    </select>
                    <input
                        type="month"
                        name="month"
                        value={filters.month}
                        onChange={handleFilterChange}
                    />
                    <select name="sortBy" value={filters.sortBy} onChange={handleFilterChange}>
                        <option value="title">Title</option>
                        <option value="price">Price</option>
                        <option value="date">Date</option>
                    </select>
                    <select name="sortOrder" value={filters.sortOrder} onChange={handleFilterChange}>
                        <option value="asc">Ascending</option>
                        <option value="desc">Descending</option>
                    </select>
                </div>
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