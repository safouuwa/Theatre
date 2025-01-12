import React, { useState, useEffect } from 'react';
import axios from 'axios';
import DatePicker from "react-datepicker";
import { useNavigate } from 'react-router-dom';
import "react-datepicker/dist/react-datepicker.css";
import './TheatreShows.css';

interface Venue {
  venueId: number;
  name: string;
  capacity: number;
}

interface TheatreShow {
  theatreShowId: number;
  title: string;
  description: string;
  price: number;
  venue?: Venue;
  theatreShowDates?: Array<{
    dateAndTime: string;
  }>;
}

interface FormData {
  title: string;
  description: string;
  price: string;
  venueId: string;
  showTime: Date;
  venueName: string;
  venueCapacity: string;
}

interface Toast {
  title: string;
  message: string;
  type: string;
}

const TheatreShows: React.FC = () => {
  const navigate = useNavigate();
  const [shows, setShows] = useState<TheatreShow[]>([]);
  const [venues, setVenues] = useState<Venue[]>([]);
  const [form, setForm] = useState<FormData>({ 
    title: '', 
    description: '', 
    price: '', 
    venueId: '', 
    showTime: new Date(),
    venueName: '',
    venueCapacity: ''
  });
  const [editingShow, setEditingShow] = useState<TheatreShow | null>(null);
  const [toast, setToast] = useState<Toast | null>(null);
  const [useExistingVenue, setUseExistingVenue] = useState(true);

  useEffect(() => {
    refreshData();
  }, []);

  const handleReturnDashboard = () => {
    navigate('/dashboard');
  };


  useEffect(() => {
    if (editingShow) {
      setForm({
        ...editingShow,
        showTime: editingShow.theatreShowDates?.[0]?.dateAndTime 
          ? new Date(editingShow.theatreShowDates[0].dateAndTime)
          : new Date(),
        venueId: editingShow.venue?.venueId?.toString() || '',
        venueName: editingShow.venue?.name || '',
        venueCapacity: editingShow.venue?.capacity?.toString() || '',
        price: editingShow.price.toString()
      });
      setUseExistingVenue(!!editingShow.venue?.venueId);
    } else {
      resetForm();
    }
  }, [editingShow]);

  const resetForm = () => {
    setForm({
      title: '',
      description: '',
      price: '',
      venueId: '',
      showTime: new Date(),
      venueName: '',
      venueCapacity: ''
    });
    setUseExistingVenue(true);
  };

  const refreshData = () => {
    refreshShows();
    fetchVenues();
  };

  const refreshShows = () => {
    axios.get<TheatreShow[]>('http://localhost:5097/api/v1/TheatreShow')
      .then((response) => {
        setShows(response.data);
      })
      .catch((error) => {
        console.error('Error fetching shows:', error);
      });
  };

  const fetchVenues = () => {
    axios.get<Venue[]>('http://localhost:5097/api/v1/Venue')
      .then((response) => {
        setVenues(response.data);
      })
      .catch((error) => {
        console.error('Error fetching venues:', error);
      });
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleVenueChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setForm({ ...form, venueId: e.target.value });
  };

  const handleDateChange = (date: Date | null) => {
    if (date) {
      setForm({ ...form, showTime: date });
    }
  };

  const handleDelete = (id: number) => {
    if (window.confirm('Are you sure you want to delete this show?')) {
      axios.delete(`http://localhost:5097/api/v1/TheatreShow/${id}`)
        .then(() => {
          refreshData();
          showToast("Success", "Show deleted successfully.", "success");
        })
        .catch((error) => {
          console.error('Error deleting show:', error);
          showToast("Error", "Shows with active reservations cannot be deleted.", "error");
        });
    }
  };

  const showToast = (title: string, message: string, type: string) => {
    setToast({ title, message, type });
    setTimeout(() => {
      setToast(null);
    }, 5000);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    const payload = {
      title: form.title,
      description: form.description,
      price: parseFloat(form.price),
      theatreShowDates: [
        {
          dateAndTime: form.showTime.toISOString()
        }
      ],
      venue: useExistingVenue 
        ? { venueId: parseInt(form.venueId) }
        : { 
            name: form.venueName,
            capacity: parseInt(form.venueCapacity)
          }
    };

    if (editingShow) {
      axios.put(`http://localhost:5097/api/v1/TheatreShow/${editingShow.theatreShowId}`, payload)
        .then(() => {
          setEditingShow(null);
          resetForm();
          refreshData();
          showToast("Success", "Show updated successfully.", "success");
        })
        .catch((error) => {
          console.error('Error updating show:', error);
          showToast("Error", "Failed to update show. Please try again.", "error");
        });
    } else {
      axios.post('http://localhost:5097/api/v1/TheatreShow', payload)
        .then(() => {
          resetForm();
          refreshData();
          showToast("Success", "Show added successfully.", "success");
        })
        .catch((error) => {
          console.error('Error adding show:', error);
          showToast("Error", "Failed to add show. Please try again.", "error");
        });
    }
  };

  const handleCancelEdit = () => {
    setEditingShow(null);
    resetForm();
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
          <div className="venue-selection">
            <label className="switch">
              <input
                type="checkbox"
                checked={useExistingVenue}
                onChange={() => setUseExistingVenue(!useExistingVenue)}
              />
              <span className="slider round"></span>
            </label>
            <span>{useExistingVenue ? "Select Existing Venue" : "Add New Venue"}</span>
          </div>
          {useExistingVenue ? (
            <select
              value={form.venueId}
              onChange={handleVenueChange}
              className="theatre-shows-select"
              required
            >
              <option value="">Select a venue</option>
              {venues.map((venue) => (
                <option key={venue.venueId} value={venue.venueId}>
                  {venue.name} - {`capacity: ${venue.capacity}`}
                </option>
              ))}
            </select>
          ) : (
            <>
              <input
                name="venueName"
                value={form.venueName}
                onChange={handleChange}
                placeholder="Venue Name"
                required
                className="theatre-shows-input"
              />
              <input
                name="venueCapacity"
                value={form.venueCapacity}
                onChange={handleChange}
                placeholder="Venue Capacity"
                required
                type="number"
                className="theatre-shows-input"
              />
            </>
          )}
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
              required
            />
          </div>
          <div className="form-buttons">
            <button type="submit" className="theatre-shows-button">
              {editingShow ? 'Update Show' : 'Add Show'}
            </button>
            <button onClick={handleReturnDashboard} className="return-home-button">
              Return
            </button>
            {editingShow && (
              <button type="button" onClick={handleCancelEdit} className="theatre-shows-button cancel">
                Cancel Edit
              </button>
            )}
          </div>
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
                <tr key={show.theatreShowId}>
                  <td>{show.title}</td>
                  <td>{show.description}</td>
                  <td>{show.price}</td>
                  <td>{show.venue?.name || 'N/A'}</td>
                  <td>
                    {show.theatreShowDates?.[0]?.dateAndTime
                      ? new Date(show.theatreShowDates[0].dateAndTime).toLocaleString()
                      : 'N/A'}
                  </td>
                  <td>
                    <div className="action-buttons">
                      <button 
                        className="theatre-shows-button edit" 
                        onClick={() => setEditingShow(show)}
                      >
                        Edit
                      </button>
                      <button 
                        className="theatre-shows-button delete" 
                        onClick={() => handleDelete(show.theatreShowId)}
                      >
                        Delete
                      </button>
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

