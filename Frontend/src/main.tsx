import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter, Route, Routes } from 'react-router-dom'


import Index from './pages/index'
import EmailManager from './pages/klant/underOverview' // Ensure this line is correct

import './styles/index.css'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Index />} />
        <Route path="/email-manager" element={<EmailManager />} />
      </Routes>
    </BrowserRouter>
  </StrictMode>,
)
