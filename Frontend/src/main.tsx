import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter, Route, Routes } from "react-router-dom";

import Index from "./pages/index";
import VoertuigenOverview from "./pages/voertuigenOverview";
import EmailManager from "./pages/klant/underOverview"; 
import NavBar from "./components/navBar";
import LoginRegister from "./pages/loginRegister";
import Abonnementen from "./pages/abonnementen";
import VerhuurAanvraag from "./pages/verhuurAanvraag";
import BackOfficeVerhuurAanvragen from "./pages/backOfficeVerhuurAanvragen";
import AdminPanel from "./pages/admin";
import FrontOfficeDashboard from "./pages/frontofficedashboard";
import VerhuurAanvragenStatus from './pages/klantAanvragen';
import GegevensPagina from "./pages/GegevensPagina";
import Footer from "./components/footer";
import Privacy from "./pages/privacy";
import PrivacyEdit from "./pages/privacyEdit";
import "./styles/index.css";
import BackOfficeAbonnementAanvragen from "./pages/backOfficeAbonnementAanvragen";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <BrowserRouter>
      <div className="main-content">
        <NavBar />
        <Routes>
          <Route path="/" element={<Index />} />
          <Route index element={<Index />} />
          <Route path="/email-manager" element={<EmailManager />} />
          <Route path="/voertuigenOverview" element={<VoertuigenOverview />} />
          <Route path="/verhuurAanvraag/:voertuigID/:voertuigNaam/:vastartdate/:vaenddate" element={<VerhuurAanvraag />} />
          <Route path="/backOfficeVerhuurAanvragen" element={<BackOfficeVerhuurAanvragen />} />
          <Route path="/auth" element={<LoginRegister />} />
          <Route path="/abonnementen" element={<Abonnementen />} />
          <Route path="/backOfficeAbonnementAanvragen" element={<BackOfficeAbonnementAanvragen />} />
          <Route path="/admin" element={<AdminPanel />} />
          <Route path="/frontofficedashboard" element={<FrontOfficeDashboard />} />
          <Route path="/verhuur-aanvraag-status" element={<VerhuurAanvragenStatus />} />
          <Route path="/profile" element={<GegevensPagina />} />
        </Routes>
      </div>
      <Footer />
      <Routes>
        <Route path="/privacy" element={<Privacy />} />
        <Route path="/privacy/edit" element={<PrivacyEdit />} />
      </Routes>
    </BrowserRouter>
  </StrictMode>
);