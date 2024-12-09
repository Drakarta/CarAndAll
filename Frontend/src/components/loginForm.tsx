import { useState } from "react";

import { useTokenStore } from "../stores";
import bcrypt from "bcryptjs";

import "../styles/loginRegisterForm.css";

export default function LoginForm() {
  const setToken = useTokenStore((state) => state.setToken);
  const [input, setInput] = useState({ email: "", password: "" });
  const handleChange = (e: { target: { name: any; value: any } }) => {
    setInput({ ...input, [e.target.name]: e.target.value });
  };


  const handleSubmit = async () => {
    const password = bcrypt.hashSync(input.password, import.meta.env.SALT)


    const response = await fetch(`${import.meta.env.VITE_REACT_APP_EMAIL_API_URL}/Login`, {
      method: 'POST',
      headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${import.meta.env.VITE_REACT_APP_EMAIL_API_KEY}`
      },
      body: JSON.stringify({ email: input.email, password })
    })

    response.json

    // setToken("Token");

    // window.location.href = "/";
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