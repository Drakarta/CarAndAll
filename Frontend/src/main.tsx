import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter, Route, Routes } from "react-router-dom";

import Index from "./pages/index";
import VoertuigenOverview from "./pages/voertuigenOverview";
import WagenParkBeheerder from "./pages/klant/underOverview"; 

import "./styles/index.css";
import NavBar from "./components/navBar";
import LoginRegister from "./pages/loginRegister";
import Abonnementen from "./pages/abonnementen";

import VerhuurAanvraag from "./pages/verhuurAanvraag";
import BackOfficeVerhuurAanvragen from "./pages/backOfficeVerhuurAanvragen";

import AdminPanel from "./pages/admin";
import FrontOfficeDashboard from "./pages/frontofficedashboard";

import VerhuurAanvragenStatus from './pages/klantAanvragen';

import Profile from "./pages/GegevensPagina";


createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<NavBar />}>
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
          <Route path="/profile" element={<Profile />} />
        </Route>
      </Routes>
    </BrowserRouter>
  </StrictMode>
);