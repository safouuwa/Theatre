import React, { useState, useEffect } from 'react';
import axios from 'axios';

function TheatreShows() {
    const [shows, setShows] = useState([]);
    const [form, setForm] = useState({ title: '', description: '', date: '', time: '' });
    const [editingShow, setEditingShow] = useState(null);

    useEffect(() => {
        axios.get('/api/v1/TheatreShow').then((response) => setShows(response.data));
        }, []);

    const handleChange = (e) =>
        setForm({ ...form, [e.target.name]: e.target.value });
    
    const handleSubmit = () => {
        if (editingShow) {
            axios.put('/api/v1/TheatreShow/update', { ...editingShow, ...form }).then(() => {
                setEditingShow(null);
                setForm({ title: '', description: '', price: 0 });
                refreshShows();
            });
        } else {
            axios.post('/api/v1/TheatreShow', form).then(() => {
                setForm({ title: '', description: '', price: 0 });
                refreshShows();
            });
        }
    };

    const refreshShows = () => {
        axios.get('/api/v1/TheatreShow').then((response) => setShows(response.data));
    };

    const handleDelete = (id) => {
        if (window.confirm('Are you sure you want to delete this show?')) {
            axios.delete(`/api/v1/TheatreShow/${id}`).then(() => refreshShows());
        }
    };

    return (
        <div>
            <h1>Theatre Shows</h1>
            <form onSubmit={(e) => { e.preventDefault(); handleSubmit(); }}>
                <input name="title" value={form.title} onChange={handleChange} placeholder="Title" required />
                <input name="description" value={form.description} onChange={handleChange} placeholder="Description" required />
                <input name="price" value={form.price} onChange={handleChange} placeholder="Price" required type="number" />
                <button type="submit">{editingShow ? 'Update Show' : 'Add Show'}</button>
            </form>

            <table>
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Description</th>
                        <th>Price</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {shows.map((show) => (
                        <tr key={show.id}>
                            <td>{show.title}</td>
                            <td>{show.description}</td>
                            <td>{show.price}</td>
                            <td>
                                <button onClick={() => { setEditingShow(show); setForm(show); }}>Edit</button>
                                <button onClick={() => handleDelete(show.id)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default TheatreShows;