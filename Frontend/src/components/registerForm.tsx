import { useState } from "react"

import "../styles/loginRegisterForm.css"

export default function RegisterForm() {
  const [input, setInput] = useState({ username: "", password: "", repeatPassword: "" })
  const handleChange = (e: { target: { name: any; value: any } }) => {
    setInput({ ...input, [e.target.name]: e.target.value })
  }
  const handleSubmit = async () => {
    if (input.password !== input.repeatPassword) {
      alert("Passwords do not match")
      return
    }
    if (input.password.length < 4) {
      alert("Password must be at least 4 characters long")
      return
    }
    if (input.username.length < 4) {
      alert("Username must be at least 4 characters long")
      return
    }
    window.location.href = "/auth"
  }

  return (
    <form className={"auth-form"} >
      <h1>Register</h1>
      <input 
        className={"input"} 
        type="text" 
        name="username"
        value={input.username}
        placeholder="Username" 
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
      <input 
        className={"input"} 
        type="password" 
        name="repeatPassword"
        value={input.repeatPassword}
        placeholder="Repeat password"
        onChange={handleChange}
      />
      <button 
        className={"button"} 
        onClick={handleSubmit}
      >
        Register
      </button>
    </form>
  )
}