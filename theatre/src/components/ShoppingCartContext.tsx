import React, { createContext, useContext, useState, useEffect } from 'react';

interface CartItem {
  theatreShowDateId: number;
  numberOfTickets: number;
  showTitle: string;
  showDate: string;
  price: number;
}

interface CustomerInfo {
  firstName: string;
  lastName: string;
  email: string;
  password?: string;
}

interface ShoppingCartContextType {
  cartItems: CartItem[];
  addToCart: (item: CartItem) => void;
  removeFromCart: (theatreShowDateId: number) => void;
  clearCart: () => void;
  customerInfo: CustomerInfo | null;
  setCustomerInfo: (info: CustomerInfo) => void;
}

const ShoppingCartContext = createContext<ShoppingCartContextType | null>(null);

export const useShoppingCart = () => {
  const context = useContext(ShoppingCartContext);
  if (!context) {
    throw new Error('useShoppingCart must be used within a ShoppingCartProvider');
  }
  return context;
};

export const ShoppingCartProvider = (props) => {
  const [cartItems, setCartItems] = useState<CartItem[]>([]);
  const [customerInfo, setCustomerInfo] = useState<CustomerInfo | null>(null);

  useEffect(() => {
    const savedCart = localStorage.getItem('shoppingCart');
    const savedCustomerInfo = localStorage.getItem('customerInfo');
    if (savedCart) {
      setCartItems(JSON.parse(savedCart));
    }
    if (savedCustomerInfo) {
      setCustomerInfo(JSON.parse(savedCustomerInfo));
    }
  }, []);

  useEffect(() => {
    localStorage.setItem('shoppingCart', JSON.stringify(cartItems));
  }, [cartItems]);

  useEffect(() => {
    if (customerInfo) {
      localStorage.setItem('customerInfo', JSON.stringify(customerInfo));
    }
  }, [customerInfo]);

  const addToCart = (item: CartItem) => {
    setCartItems(prevItems => {
      const existingItemIndex = prevItems.findIndex(i => i.theatreShowDateId === item.theatreShowDateId);
      if (existingItemIndex > -1) {
        const updatedItems = [...prevItems];
        updatedItems[existingItemIndex].numberOfTickets += item.numberOfTickets;
        return updatedItems;
      }
      return [...prevItems, item];
    });
  };

  const removeFromCart = (theatreShowDateId: number) => {
    setCartItems(prevItems => {
      const updatedItems = prevItems.filter(item => item.theatreShowDateId !== theatreShowDateId);
      if (updatedItems.length === 0) {
        setCustomerInfo(null);
        localStorage.removeItem('customerInfo');
      }
      return updatedItems;
    });
  };

  const clearCart = () => {
    setCartItems([]);
    setCustomerInfo(null);
    localStorage.removeItem('shoppingCart');
    localStorage.removeItem('customerInfo');
  };

  return (
    <ShoppingCartContext.Provider value={{ cartItems, addToCart, removeFromCart, clearCart, customerInfo, setCustomerInfo }}>
      {props.children}
    </ShoppingCartContext.Provider>
  );
};

