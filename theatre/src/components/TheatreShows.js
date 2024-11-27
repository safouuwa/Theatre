import React, { useState, useEffect } from 'react';
import axios from 'axios';

function TheatreShows() {
    const [shows, setShows] = useState([
        { id: 1, title: "The Phantom of the Opera", description: "A haunting musical.", price: 100 },
        { id: 2, title: "Hamilton", description: "A story of the American Founding Father.", price: 150 },
        { id: 3, title: "Les Misérables", description: "A tale of revolution and redemption.", price: 120 }
    ]);
    const [form, setForm] = useState({ title: '', description: '', date: '', time: '' });
    const [editingShow, setEditingShow] = useState(null);

// add the greyed out code and remove the hardcoded shows array to fetch data from the API

//    useEffect(() => {
//        axios.get('/api/v1/TheatreShow').then((response) => setShows(response.data))
//        .catch((error) => { 
//            console.error('Error fetching data: ' + error);
//        });
//    }, []);

    const handleChange = (e) =>
        setForm({ ...form, [e.target.name]: e.target.value });
    
    const handleSubmit = () => {
        if (editingShow) {
            // Update the existing show
            setShows((prev) =>
                prev.map((show) =>
                    show.id === editingShow.id ? { ...show, ...form } : show
                )
            );
            setEditingShow(null);
        } else {
            // Add a new show
            const newShow = { id: Date.now(), ...form }; // Use Date.now() as a temporary ID
            setShows((prev) => [...prev, newShow]);
        }
        setForm({ title: '', description: '', price: 0 }); // Clear the form
    };

//    const handleSubmit = () => {
//        if (editingShow) {
//            axios.put('/api/v1/TheatreShow/update', { ...editingShow, ...form }).then(() => {
//                setEditingShow(null);
//                setForm({ title: '', description: '', price: 0 });
//                refreshShows();
//            });
//        } else {
//            axios.post('/api/v1/TheatreShow', form).then(() => {
//                setForm({ title: '', description: '', price: 0 });
//                refreshShows();
//            });
//        }
//    };
//
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
            {shows.length > 0 && (
                <table className="theatre-shows-table">
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
                                <td className="action-buttons">
                                    <button onClick={() => { setEditingShow(show); setForm(show); }}>Edit</button>
                                    <button className="delete-button" onClick={() => handleDelete(show.id)}>Delete</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}

export default TheatreShows;