import React, { createContext, useContext, useState, useEffect } from 'react';
import axios from 'axios';

interface CustomerData {
  customerId: number;
  firstName: string;
  lastName: string;
  email: string;
}

interface AuthContextType {
  isAuthenticated: boolean;
  isAdmin: boolean;
  customerData: CustomerData | null;
  login: (username: string, password: string) => Promise<boolean>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | null>(null);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isAdmin, setIsAdmin] = useState(false);
  const [customerData, setCustomerData] = useState<CustomerData | null>(null);

  const checkAdminStatus = async () => {
    try {
      await axios.get('http://localhost:5097/api/v1/Login/IsAdminLoggedIn');
      setIsAdmin(true);
      return true;
    } catch {
      setIsAdmin(false);
      return false;
    }
  };

  const login = async (username: string, password: string): Promise<boolean> => {
    try {
      const response = await axios.post('http://localhost:5097/api/v1/Login/Login', { username, password });
      setIsAuthenticated(true);
      
      const adminStatus = await checkAdminStatus();
      if (!adminStatus) {
        // Fetch customer data if not admin
        const customerResponse = await axios.get('http://localhost:5097/api/v1/Login/CustomerData');
        setCustomerData(customerResponse.data);
      }
      return adminStatus;
    } catch (error) {
      console.error('Login error:', error);
      return false;
    }
  };

  const logout = async () => {
    try {
      await axios.get('http://localhost:5097/api/v1/Login/Logout');
      setIsAuthenticated(false);
      setIsAdmin(false);
      setCustomerData(null);
    } catch (error) {
      console.error('Logout error:', error);
    }
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, isAdmin, customerData, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

