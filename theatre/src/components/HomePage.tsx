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
    const [venues, setVenues] = useState<string[]>([]);
    const [filters, setFilters] = useState({
        title: '',
        description: '',
        venue: '',
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
        axios.get<string[]>('http://localhost:5097/api/v1/Venue')
            .then(response => {
                setVenues(response.data);
            })
            .catch(error => {
                console.error('Error fetching venues:', error);
            });
    }, []);

    useEffect(() => {
        const { title, description, venue, month, sortBy, sortOrder } = filters;
        let url = `http://localhost:5097/api/v1/TheatreShow/filter/${sortBy}/${sortOrder}?`;

        if (title) url += `title=${title}&`;
        if (description) url += `description=${description}&`;
        if (venue) url += `location=${venue}&`;
        if (month) {
            const startDate = new Date(month);
            const endDate = new Date(startDate.getFullYear(), startDate.getMonth() + 1, 0);
            url += `startDate=${startDate.toISOString()}&endDate=${endDate.toISOString()}&`;
        }

        axios.get<TheatreShow[]>(url)
            .then(response => {
                setShows(response.data);
            })
            .catch(error => {
                console.error('Error fetching shows:', error);
            });
    }, [filters]);

    const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setFilters(prevFilters => ({ ...prevFilters, [name]: value }));
    };

    return (
        <div className="home-page">
            <div className="filters">
                <input
                    type="text"
                    name="title"
                    value={filters.title}
                    onChange={handleFilterChange}
                    placeholder="Search by title"
                />
                <input
                    type="text"
                    name="description"
                    value={filters.description}
                    onChange={handleFilterChange}
                    placeholder="Search by description"
                />
                <select name="venue" value={filters.venue} onChange={handleFilterChange}>
                    <option value="">Select a venue</option>
                    {venues.map(venue => (
                        <option key={venue} value={venue}>{venue}</option>
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
                    <option value="description">Description</option>
                    <option value="price">Price</option>
                    <option value="date">Date</option>
                </select>
                <select name="sortOrder" value={filters.sortOrder} onChange={handleFilterChange}>
                    <option value="asc">Ascending</option>
                    <option value="desc">Descending</option>
                </select>
            </div>
            <div className="shows-list">
                {shows.map(show => (
                    <div key={show.theatreShowId} className="show-item">
                        <h3>{show.title}</h3>
                        <p>{show.description}</p>
                        <p>{show.price}</p>
                        <p>{show.venue.name}</p>
                        <p>{new Date(show.theatreShowDates[0].dateAndTime).toLocaleString()}</p>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default HomePage;