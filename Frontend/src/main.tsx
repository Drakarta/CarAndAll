import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter, Route, Routes } from 'react-router-dom'

import Index from './pages/index'
import LoginRegister from './pages/loginRegister'

import './styles/index.css'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        {/* Hier maak je meer paginas */}
        {/* reactrouter.com */}
        <Route path="/" element={<Index />} />'
        <Route path="/auth" element={<LoginRegister />} />
        <Route path="/test">
          <Route index element={<h1>Test</h1>} />
          <Route path="/pagina" element={<h1>Test </h1>} />
        </Route>
      </Routes>
    </BrowserRouter>
  </StrictMode>,
)
