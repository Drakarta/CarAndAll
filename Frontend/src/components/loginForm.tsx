import { useState } from "react";

import { useTokenStore } from "../stores";

import "../styles/loginRegisterForm.css";

export default function LoginForm() {
  const setToken = useTokenStore((state) => state.setToken);
  const [input, setInput] = useState({ email: "", password: "" });
  const handleChange = (e: { target: { name: any; value: any } }) => {
    setInput({ ...input, [e.target.name]: e.target.value });
  };
  
  const handleSubmit = async (event: any) => {
    event.preventDefault()
    const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/account/login`, {
      method: 'POST',
      headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${import.meta.env.VITE_REACT_APP_EMAIL_API_KEY}`
      },
      body: JSON.stringify({ email: input.email, password: input.password })
    })

    if (response.status == 200) {
      const data = await response.json()
      setToken(data.userId, data.role)
      alert("Successfully logged in!")
      window.location.href = "/"
    } else if (response.status == 400) {
      alert("Check if all fields are filled in.")
    } else if (response.status == 401) {
      alert("Wrong credetials please try again.")
    }
  }

  return (
    <div>
      <form className="auth-form">
        <h1>Login</h1>
        <input 
          className={"input"} 
          type="text" 
          name="email"
          value={input.email}
          placeholder="Email@example.com" 
          onChange={handleChange} 
        />
        <input 
          className={"input"} 
          type="password" 
          name="password"
          value={input.password} 
          placeholder="Password"
          onChange={handleChange}
        />
        <button 
          className={"button"} 
          onClick={handleSubmit}
        >
          Login
        </button>
      </form>
    </div>
  )
}