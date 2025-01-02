import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
// import type { ReservationFormData, ReservationRequest } from '../types/reservation';
import './ReservationForm.css';

interface ReservationFormProps {
  showId: number;
  theatreShowDateId: number;
  onCancel: () => void;
}

interface ReservationRequest {
    FirstName: string;
    LastName: string;
    Email: string;
    Requests: {
      TheatreShowDateId: number;
      NumberOfTickets: number;
    }[];
  }
  
interface ReservationFormData {
    firstName: string;
    lastName: string;
    email: string;
    numberOfTickets: number;
  }

const ReservationForm: React.FC<ReservationFormProps> = ({ showId, theatreShowDateId, onCancel }) => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<ReservationFormData>({
    firstName: '',
    lastName: '',
    email: '',
    numberOfTickets: 1
  });
  const [error, setError] = useState<string>('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'numberOfTickets' ? parseInt(value) || 0 : value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsSubmitting(true);

    const reservationData: ReservationRequest = {
      FirstName: formData.firstName,
      LastName: formData.lastName,
      Email: formData.email,
      Requests: [{
        TheatreShowDateId: theatreShowDateId,
        NumberOfTickets: formData.numberOfTickets
      }]
    };

    try {
      await axios.post('http://localhost:5097/api/v1/Reservation', reservationData);
      alert('Reservation successful!');
      navigate('/');
    } catch (error) {
      setError('Failed to make reservation. Please try again.');
      console.error('Reservation error:', error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="reservation-form-container">
      <h2>Make a Reservation</h2>
      <form onSubmit={handleSubmit} className="reservation-form">
        <div className="form-group">
          <label htmlFor="firstName">First Name:</label>
          <input
            type="text"
            id="firstName"
            name="firstName"
            value={formData.firstName}
            onChange={handleChange}
            required
            className="form-input"
          />
        </div>

        <div className="form-group">
          <label htmlFor="lastName">Last Name:</label>
          <input
            type="text"
            id="lastName"
            name="lastName"
            value={formData.lastName}
            onChange={handleChange}
            required
            className="form-input"
          />
        </div>

        <div className="form-group">
          <label htmlFor="email">Email:</label>
          <input
            type="email"
            id="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            required
            className="form-input"
          />
        </div>

        <div className="form-group">
          <label htmlFor="numberOfTickets">Number of Tickets:</label>
          <input
            type="number"
            id="numberOfTickets"
            name="numberOfTickets"
            value={formData.numberOfTickets}
            onChange={handleChange}
            min="1"
            required
            className="form-input"
          />
        </div>

        {error && <div className="error-message">{error}</div>}

        <div className="form-buttons">
          <button type="submit" disabled={isSubmitting} className="submit-button">
            {isSubmitting ? 'Submitting...' : 'Confirm Reservation'}
          </button>
          <button type="button" onClick={onCancel} className="cancel-button">
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
};

export default ReservationForm;

