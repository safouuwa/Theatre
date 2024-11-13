import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom'; 
import './Login.css'; 

function Login() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const navigate = useNavigate();

  const handleEnter = (event) => {
    if (event.key === 'Enter') {
      handleLogin()
    }
  };

  const handleLogin = () => {
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
        <label>Username </label>
        <input
          type="text"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          onKeyDown={handleEnter}
        />
      </div>

      {/* Password input */}
      <div>
        <label>Password </label>
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          onKeyDown={handleEnter}
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
