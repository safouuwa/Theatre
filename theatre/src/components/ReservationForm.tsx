import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './AuthContext.tsx';
import './ReservationForm.css';

interface ReservationFormProps {
  showId: number;
  theatreShowDateId: number;
  onCancel: () => void;
}

interface NonLoggedReservationRequest {
  FirstName: string;
  LastName: string;
  Email: string;
  Password?: string;
  Requests: LoggedReservationRequest[];
}

interface LoggedReservationRequest {
  TheatreShowDateId: number;
  NumberOfTickets: number;
}

const ReservationForm: React.FC<ReservationFormProps> = ({ showId, theatreShowDateId, onCancel }) => {
  const navigate = useNavigate();
  const { isAuthenticated, customerData } = useAuth();
  const [formData, setFormData] = useState({
    firstName: customerData?.firstName || '',
    lastName: customerData?.lastName || '',
    email: customerData?.email || '',
    password: '',
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

    const reservationRequest: LoggedReservationRequest = {
      TheatreShowDateId: theatreShowDateId,
      NumberOfTickets: formData.numberOfTickets
    };

    try {
      if (isAuthenticated) {
        await axios.post('http://localhost:5097/api/v1/Reservation/account', [reservationRequest]);
      } else {
        const guestReservationRequest: NonLoggedReservationRequest = {
          FirstName: formData.firstName,
          LastName: formData.lastName,
          Email: formData.email,
          Password: formData.password,
          Requests: [reservationRequest]
        };
        await axios.post('http://localhost:5097/api/v1/Reservation', guestReservationRequest);
      }
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
        {!isAuthenticated && (
          <>
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
              <label htmlFor="password">Password (optional):</label>
              <input
                type="password"
                id="password"
                name="password"
                value={formData.password}
                onChange={handleChange}
                className="form-input"
              />
            </div>
          </>
        )}

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

