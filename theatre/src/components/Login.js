import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom'; 
import axios from 'axios';
import './Login.css'; 

function Login() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const navigate = useNavigate();

  const handleLogin = () => {
    axios.post('http://localhost:5097/api/v1/Login/Login', { username, password })
      .then(() => {
        setSuccessMessage('Login successful! Redirecting to dashboard...');
        setTimeout(() => navigate('/dashboard'), 1000);
      })
      .catch((error) => {
        if (error.response.status === 401) {
          setErrorMessage('Invalid username or password');
        } else {
          setErrorMessage('An error occurred. Please try again later.');
        }
      });
    };
  return (
    <div className="login-container">
      <h2>Login</h2>

      {/* Username input */}
      <div>
        <label>Username </label>
        <input
          type="text"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
        />
      </div>

      {/* Password input */}
      <div>
        <label>Password </label>
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
      </div>

      {/* Error or success message */}
      {errorMessage && <div className="error-message">{errorMessage}</div>}
      {successMessage && <div className="success-message">{successMessage}</div>}

      {/* Login button */}
      <button onClick={handleLogin}>Login</button>
    </div>
  );
}
export default Login;
