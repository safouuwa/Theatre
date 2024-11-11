import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom'; 
import './Login.css'; // Import the CSS for this component

function Login() {
  // State to keep track of user input and the error message
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const navigate = useNavigate();

  // Simulate login function
  const handleLogin = () => {
    // Clear previous success or error messages
    setErrorMessage('');
    setSuccessMessage('');

    // Simulate login logic (this will be replaced with backend logic later)
    if (username === 'admin' && password === 'password') {
      setSuccessMessage('Login successful!');
      navigate('/dashboard');
    } else {
      setErrorMessage('Invalid username or password.');
    }
  };

  return (
    <div className="login-container">
      <h2>Login</h2>

      {/* Username input */}
      <div>
        <label>Username</label>
        <input
          type="text"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
        />
      </div>

      {/* Password input */}
      <div>
        <label>Password</label>
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
