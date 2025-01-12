import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './AuthContext.tsx';
import { useShoppingCart } from './ShoppingCartContext.tsx';
// import './ShoppingCart.css';
interface ReservationRequest {
    FirstName: string;
    LastName: string;
    Email: string;
    Password?: string;
    Requests: {
      TheatreShowDateId: number;
      NumberOfTickets: number;
    }[];
  }
  
  const ShoppingCart = () => {
    const navigate = useNavigate();
    const { isAuthenticated } = useAuth();
    const { cartItems, removeFromCart, clearCart, customerInfo } = useShoppingCart();
    const [error, setError] = useState<string>('');
    const [isSubmitting, setIsSubmitting] = useState(false);
  
    const handleRemoveItem = (theatreShowDateId: number) => {
      removeFromCart(theatreShowDateId);
    };
  
    const handleCheckout = async () => {
      setError('');
      setIsSubmitting(true);
  
      try {
        if (isAuthenticated) {
          const requests = cartItems.map(item => ({
            TheatreShowDateId: item.theatreShowDateId,
            NumberOfTickets: item.numberOfTickets
          }));
          await axios.post('http://localhost:5097/api/v1/Reservation/account', requests);
        } else {
          if (!customerInfo) {
            throw new Error('Customer information is missing');
          }
  
          const reservationRequest: ReservationRequest = {
            FirstName: customerInfo.firstName,
            LastName: customerInfo.lastName,
            Email: customerInfo.email,
            ...(customerInfo.password && { Password: customerInfo.password }),
            Requests: cartItems.map(item => ({
              TheatreShowDateId: item.theatreShowDateId,
              NumberOfTickets: item.numberOfTickets
            }))
          };
  
          await axios.post('http://localhost:5097/api/v1/Reservation', reservationRequest);
        }
        alert('Reservation successful!');
        clearCart();
        navigate('/');
      } catch (error) {
        setError('Failed to make reservation. Please try again.');
        console.error('Reservation error:', error);
      } finally {
        setIsSubmitting(false);
      }
    };
  
    const handleReturnHome = () => {
      navigate('/');
    };
  
    const totalPrice = cartItems.reduce((total, item) => total + item.price * item.numberOfTickets, 0);
  
    return (
      <div className="shopping-cart-container">
        <h2>Shopping Cart</h2>
        <button onClick={handleReturnHome} className="return-home-button">
          Return to Home
        </button>
        {cartItems.length === 0 ? (
          <p>Your cart is empty.</p>
        ) : (
          <>
            <ul className="cart-items">
              {cartItems.map(item => (
                <li key={item.theatreShowDateId} className="cart-item">
                  <div>
                    <h3>{item.showTitle}</h3>
                    <p>Date: {new Date(item.showDate).toLocaleString()}</p>
                    <p>Tickets: {item.numberOfTickets}</p>
                    <p>Price: ${(item.price * item.numberOfTickets).toFixed(2)}</p>
                  </div>
                  <button onClick={() => handleRemoveItem(item.theatreShowDateId)} className="remove-button">
                    Remove
                  </button>
                </li>
              ))}
            </ul>
            <div className="cart-summary">
              <p>Total: ${totalPrice.toFixed(2)}</p>
              {!isAuthenticated && customerInfo && (
                <div className="customer-info">
                  <h3>Customer Information:</h3>
                  <p>Name: {customerInfo.firstName} {customerInfo.lastName}</p>
                  <p>Email: {customerInfo.email}</p>
                </div>
              )}
              <button onClick={handleCheckout} disabled={isSubmitting} className="checkout-button">
                {isSubmitting ? 'Processing...' : 'Checkout'}
              </button>
            </div>
            {error && <div className="error-message">{error}</div>}
          </>
        )}
      </div>
    );
  };
  
  export default ShoppingCart;