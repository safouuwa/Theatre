import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './AuthContext.tsx';
import { useShoppingCart } from './ShoppingCartContext.tsx';
import './ReservationForm.css';

interface ReservationFormProps {
  showId: number;
  theatreShowDateId: number;
  showTitle: string;
  showDate: string;
  price: number;
  onCancel: () => void;
}

const ReservationForm: React.FC<ReservationFormProps> = ({ 
  showId, 
  theatreShowDateId, 
  showTitle, 
  showDate, 
  price, 
  onCancel 
}) => {
  const navigate = useNavigate();
  const { isAuthenticated, customerData } = useAuth();
  const { addToCart, customerInfo, setCustomerInfo, cartItems } = useShoppingCart();
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    numberOfTickets: 1
  });
  const [error, setError] = useState<string>('');

  useEffect(() => {
    if (isAuthenticated && customerData) {
      setFormData(prev => ({
        ...prev,
        firstName: customerData.firstName || '',
        lastName: customerData.lastName || '',
        email: customerData.email || ''
      }));
    } else if (customerInfo && cartItems.length > 0) {
      setFormData(prev => ({
        ...prev,
        firstName: customerInfo.firstName,
        lastName: customerInfo.lastName,
        email: customerInfo.email
      }));
    } else {
      setFormData(prev => ({
        ...prev,
        firstName: '',
        lastName: '',
        email: '',
        password: ''
      }));
    }
  }, [isAuthenticated, customerData, customerInfo, cartItems]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'numberOfTickets' ? parseInt(value) || 0 : value
    }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!isAuthenticated && !customerInfo) {
      setCustomerInfo({
        firstName: formData.firstName,
        lastName: formData.lastName,
        email: formData.email,
        password: formData.password
      });
    }

    addToCart({
      theatreShowDateId,
      numberOfTickets: formData.numberOfTickets,
      showTitle,
      showDate,
      price
    });

    navigate('/cart');
  };

  return (
    <div className="reservation-form-container">
      <h2>Add to Cart</h2>
      <form onSubmit={handleSubmit} className="reservation-form">
        {!isAuthenticated && (cartItems.length === 0 || !customerInfo) && (
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
          <button type="submit" className="submit-button">
            Add to Cart
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

