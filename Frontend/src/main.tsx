import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter, Route, Routes } from 'react-router-dom'


import Index from "./pages/index";
import VoertuigenOverview from "./pages/voertuigenOverview";
import EmailManager from './pages/klant/underOverview' // Ensure this line is correct

import "./styles/index.css";
import NavBar from "./components/navBar";
import LoginRegister from './pages/loginRegister';

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<NavBar />}>
          <Route index element={<Index />} />
          <Route path="/email-manager" element={<EmailManager />} />
          <Route path="/voertuigenOverview" element={<VoertuigenOverview />} />
          <Route path="/auth" element={<LoginRegister />} />
        </Route>
      </Routes>
    </BrowserRouter>
  </StrictMode>
);
