import { BrowserRouter, Route, Routes } from 'react-router-dom'
import Index from './pages'
import LoginRegister from './pages/loginRegister'
import Abonnementen from './pages/abonnementen';
import Frontofficedashboard from './pages/frontofficedashboard';
import VerhuurAanvragenStatus from './pages/klantAanvragen';


export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Hier maak je meer paginas */}
        {/* reactrouter.com */}
        <Route path="/" element={<Index />} />
        <Route path="/auth" element={<LoginRegister />} />
        <Route path="/abonnementen" element={<Abonnementen />} />
        <Route path="/frontofficedashboard" element={<Frontofficedashboard />} />
        <Route path="/verhuur-aanvraag-status" element={<VerhuurAanvragenStatus />} />

      </Routes>
    </BrowserRouter>
  )
}