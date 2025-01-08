import React from "react";
import styled from "styled-components";
import { useTheme } from "./Themecontext";

const Container = styled.div`
  background-color: ${(props) => props.theme.background};
  color: ${(props) => props.theme.text};
  min-height: 100vh;
  display: flex;
  flex-direction: column;
`;

const Header = styled.header`
  display: flex;
  justify-content: flex-start;
  padding: 10px 20px;
`;

const ToggleButton = styled.button`
  background: ${(props) => props.theme.buttonBackground};
  color: ${(props) => props.theme.buttonText};
  border: none;
  padding: 10px 20px;
  cursor: pointer;
  border-radius: 5px;

  &:hover {
    opacity: 0.8;
  }
`;

const Main = styled.main`
  flex: 1;
  padding: 20px;
`;

const Layout = ({ children }) => {
  const { toggleTheme } = useTheme();

  return (
    <Container>
      <Header>
        <ToggleButton onClick={toggleTheme}>Toggle Theme</ToggleButton>
      </Header>
      <Main>{children}</Main>
    </Container>
  );
};

export default Layout;
