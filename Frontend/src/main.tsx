import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter, Route, Routes } from "react-router-dom";

import Index from "./pages/index";
import VoertuigenOverview from "./pages/voertuigenOverview";
import EmailManager from "./pages/klant/underOverview"; // Ensure this line is correct

import "./styles/index.css";
import NavBar from "./components/navBar";
import LoginRegister from "./pages/loginRegister";
import Abonnementen from "./pages/abonnementen";
import FrontOfficeDashboard from "./pages/frontofficedashboard";
import VerhuurAanvragenStatus from './pages/klantAanvragen';

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<NavBar />}>
          <Route index element={<Index />} />
          <Route path="/email-manager" element={<EmailManager />} />
          <Route path="/voertuigenOverview" element={<VoertuigenOverview />} />
          <Route path="/auth" element={<LoginRegister />} />
          <Route path="/abonnementen" element={<Abonnementen />} />
          <Route path="/frontofficedashboard" element={<FrontOfficeDashboard />} />
          <Route path="/verhuur-aanvraag-status" element={<VerhuurAanvragenStatus />} />
        </Route>
      </Routes>
    </BrowserRouter>
  </StrictMode>
);