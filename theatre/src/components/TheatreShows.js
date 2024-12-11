import React, { useState, useEffect } from 'react';
import axios from 'axios';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import './TheatreShows.css';

function TheatreShows() {
    const [shows, setShows] = useState([]);
    const [venues, setVenues] = useState([]);
    const [form, setForm] = useState({ title: '', description: '', price: '', venueId: '', showTime: new Date() });
    const [editingShow, setEditingShow] = useState(null);
    const [toast, setToast] = useState(null);

    useEffect(() => {
        refreshShows();
        fetchVenues();
    }, []);

    const refreshShows = () => {
        axios.get('http://localhost:5097/api/v1/TheatreShow')
            .then((response) => setShows(response.data))
            .catch((error) => {
                console.error('Error fetching shows:', error);
                showToast("Error", "Failed to fetch shows. Please try again.", "error");
            });
    };

    const fetchVenues = () => {
        axios.get('http://localhost:3000/api/venues')
            .then((response) =>
                {
                    console.log('Venues fetched:', response.data);
                setVenues(response.data);
                })
            .catch((error) => {
                console.error('Error fetching venues:', error);
                showToast("Error", "Failed to fetch venues. Please try again.", "error");
            });
    };

    const handleChange = (e) =>
        setForm({ ...form, [e.target.name]: e.target.value });

    const handleVenueChange = (e) =>
        setForm({ ...form, venueId: e.target.value });

    const handleDateChange = (date) =>
        setForm({ ...form, showTime: date });

    const handleSubmit = (e) => {
        e.preventDefault();
        const payload = {
            ...form,
            price: parseFloat(form.price),
            venueId: parseInt(form.venueId),
            showTime: form.showTime.toISOString()
        };
        if (editingShow) {
            axios.put(`http://localhost:5097/api/v1/TheatreShow/update`, { ...editingShow, ...payload })
                .then(() => {
                    setEditingShow(null);
                    setForm({ title: '', description: '', price: '', venueId: '', showTime: new Date() });
                    refreshShows();
                    showToast("Success", "Show updated successfully.", "success");
                })
                .catch((error) => {
                    console.error('Error updating show:', error);
                    showToast("Error", "Failed to update show. Please try again.", "error");
                });
        } else {
            axios.post('http://localhost:5097/api/v1/TheatreShow', payload)
                .then(() => {
                    setForm({ title: '', description: '', price: '', venueId: '', showTime: new Date() });
                    refreshShows();
                    showToast("Success", "Show added successfully.", "success");
                })
                .catch((error) => {
                    console.error('Error adding show:', error);
                    showToast("Error", "Failed to add show. Please try again.", "error");
                });
        }
    };

    const handleDelete = (id) => {
        if (window.confirm('Are you sure you want to delete this show?')) {
            axios.delete(`http://localhost:5097/api/v1/TheatreShow/${id}`)
                .then(() => {
                    refreshShows();
                    showToast("Success", "Show deleted successfully.", "success");
                })
                .catch((error) => {
                    console.error('Error deleting show:', error);
                    showToast("Error", "Failed to delete show. Please try again.", "error");
                });
        }
    };

    const showToast = (title, message, type) => {
        setToast({ title, message, type });
        setTimeout(() => setToast(null), 3000);
    };

    return (
        <div className="theatre-shows-container">
            <div className="theatre-shows-card">
                <h2 className="theatre-shows-title">Theatre Shows</h2>
                <form onSubmit={handleSubmit} className="theatre-shows-form">
                    <input
                        name="title"
                        value={form.title}
                        onChange={handleChange}
                        placeholder="Title"
                        required
                        className="theatre-shows-input"
                    />
                    <input
                        name="description"
                        value={form.description}
                        onChange={handleChange}
                        placeholder="Description"
                        required
                        className="theatre-shows-input"
                    />
                    <input
                        name="price"
                        value={form.price}
                        onChange={handleChange}
                        placeholder="Price"
                        required
                        type="number"
                        className="theatre-shows-input"
                    />
                    <select
                        value={form.venueId}
                        onChange={handleVenueChange}
                        className="theatre-shows-select"
                        required
                    >
                        <option value="">Select a venue</option>
                        {venues.map((venue) => (
                            <option key={venue.id} value={venue.id.toString()}>
                                {venue.name}
                            </option>
                        ))}
                    </select>
                    <div className="theatre-shows-datepicker">
                        <label htmlFor="showTime" className="theatre-shows-label">Show Date and Time</label>
                        <DatePicker
                            selected={form.showTime}
                            onChange={handleDateChange}
                            showTimeSelect
                            timeFormat="HH:mm"
                            timeIntervals={15}
                            dateFormat="MMMM d, yyyy h:mm aa"
                            className="theatre-shows-input"
                        />
                    </div>
                    <button type="submit" className="theatre-shows-button">
                        {editingShow ? 'Update Show' : 'Add Show'}
                    </button>
                </form>
                {shows.length > 0 && (
                    <table className="theatre-shows-table">
                        <thead>
                            <tr>
                                <th>Title</th>
                                <th>Description</th>
                                <th>Price</th>
                                <th>Venue</th>
                                <th>Show Time</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {shows.map((show) => (
                                <tr key={show.id}>
                                    <td>{show.title}</td>
                                    <td>{show.description}</td>
                                    <td>{show.price}</td>
                                    <td>{venues.find(v => v.id === show.venueId)?.name || 'Unknown'}</td>
                                    <td>{new Date(show.showTime).toLocaleString()}</td>
                                    <td>
                                        <div className="action-buttons">
                                            <button className="theatre-shows-button edit" onClick={() => { setEditingShow(show); setForm({...show, showTime: new Date(show.showTime)}); }}>Edit</button>
                                            <button className="theatre-shows-button delete" onClick={() => handleDelete(show.id)}>Delete</button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </div>
            {toast && (
                <div className={`theatre-shows-toast ${toast.type}`}>
                    <strong>{toast.title}</strong>: {toast.message}
                </div>
            )}
        </div>
    );
}

export default TheatreShows;

