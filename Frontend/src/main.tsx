import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter, Route, Routes } from "react-router-dom";

import Index from "./pages/index";
import VoertuigenOverview from "./pages/voertuigenOverview";
import WagenParkBeheerder from "./pages/klant/underOverview";
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
import VierNullVier from "./pages/404";

import "./styles/index.css";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <BrowserRouter>
    <div className="main-content">
    <NavBar />
        <Routes>
          <Route path="/" element={<Index />} />
          <Route path="/WagenParkBeheerder" element={<WagenParkBeheerder />} />
          <Route index element={<Index />} />
          <Route path="/WagenParkBeheerder" element={<WagenParkBeheerder />} />
          <Route path="/voertuigenOverview" element={<VoertuigenOverview />} />
          <Route path="/verhuurAanvraag/:voertuigID/:voertuigNaam/:vastartdate/:vaenddate" element={<VerhuurAanvraag />} />
          <Route path="/backOfficeVerhuurAanvragen" element={<BackOfficeVerhuurAanvragen />} />
          <Route path="/auth" element={<LoginRegister />} />
          <Route path="/abonnementen" element={<Abonnementen />} />
          <Route path="/admin" element={<AdminPanel />} />
          <Route path="/frontofficedashboard" element={<FrontOfficeDashboard />} />
          <Route path="/verhuur-aanvraag-status" element={<VerhuurAanvragenStatus />} />
          <Route path="/profile" element={<GegevensPagina />} />
          <Route path="/privacy" element={<Privacy />} />
          <Route path="/privacy/edit" element={<PrivacyEdit />} />
          <Route path="/404" element={<VierNullVier />} />
          </Routes>
          </div>
          <Footer />
    </BrowserRouter>
  </StrictMode>
);