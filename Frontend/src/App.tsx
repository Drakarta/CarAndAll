import { BrowserRouter, Route, Routes } from 'react-router-dom'
import Index from './pages'
import LoginRegister from './pages/loginRegister'
import { useTokenStore } from './stores'

export default function App() {
  const token = useTokenStore(state => state.token)

  return (
    <BrowserRouter>
      <Routes>
        {/* Hier maak je meer paginas */}
        {/* reactrouter.com */}
        <Route path="/" element={<Index />} />
        <Route path="/auth" element={<LoginRegister />} />
      </Routes>
    </BrowserRouter>
  )
}
