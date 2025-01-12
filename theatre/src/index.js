import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App.tsx';
import { BrowserRouter } from 'react-router-dom'; // Import BrowserRouter
import { ThemeProvider } from "./Themecontext";

ReactDOM.render(
  <ThemeProvider>
  <BrowserRouter> {/* Wrap your app with BrowserRouter */}
    <App />
  </BrowserRouter>,
  </ThemeProvider>,
  document.getElementById('root')
);
