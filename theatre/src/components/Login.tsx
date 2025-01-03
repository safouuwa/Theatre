import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './AuthContext.tsx';
import { useShoppingCart } from './ShoppingCartContext.tsx';
import './Login.css';

const Login: React.FC = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const navigate = useNavigate();
  const { login } = useAuth();
  const { clearCart } = useShoppingCart();

  const handleLogin = async () => {
    try {
      const isAdmin = await login(username, password);
      clearCart(); // Clear cart on successful login
      if (isAdmin) {
        navigate('/dashboard');
      } else {
        navigate('/');
      }
    } catch (error: any) {
      if (error.response?.status === 401) {
        setErrorMessage('Invalid username or password');
      } else {
        setErrorMessage('An error occurred. Please try again later.');
      }
    }
  };

  return (
    <div className="login-container">
      <h2>Login</h2>
      <div>
        <label>Username </label>
        <input
          type="text"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
        />
      </div>
      <div>
        <label>Password </label>
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
      </div>
      {errorMessage && <div className="error-message">{errorMessage}</div>}
      <button onClick={handleLogin}>Login</button>
    </div>
  );
};

export default Login;