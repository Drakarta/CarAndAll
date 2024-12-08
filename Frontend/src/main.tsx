import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter, Route, Routes } from "react-router-dom";

import Index from "./pages/index";
import VoertuigenOverview from "./pages/voertuigenOverview";

import "./styles/index.css";
import NavBar from "./components/navBar";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<NavBar />}>
          <Route index element={<Index />} />
          <Route path="/voertuigenOverview" element={<VoertuigenOverview />} />
        </Route>
      </Routes>
    </BrowserRouter>
  </StrictMode>
);
