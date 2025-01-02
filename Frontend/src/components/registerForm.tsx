import { useState } from "react"
import "../styles/loginRegisterForm.css"

export default function RegisterForm() {
  const [input, setInput] = useState({ email: "", password: "", repeatPassword: "" })
  const [isEmailValid, setIsEmailValid] = useState(true)

  const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/

  const handleChange = (e: { target: { name: any; value: any } }) => {
    setInput({ ...input, [e.target.name]: e.target.value })

    if (e.target.name === "email") {
      setIsEmailValid(emailRegex.test(e.target.value))
    }
  }

  const handleSubmit = async (event: any) => {
    event.preventDefault()
    if (input.password !== input.repeatPassword) {
      alert("Passwords do not match")
      return
    }
    if (input.password.length < 4) {
      alert("Password must be at least 4 characters long")
      return
    }
    if (input.email.length < 4) {
      alert("Email must be at least 4 characters long")
      return
    }
    if (!isEmailValid) {
      alert("Please enter a valid email address.")
      return
    }

    const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/account/register`, {
      method: 'POST',
      headers: {
          'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email: input.email, password: input.password })
    })

    if (response.status == 200) {
      alert("Successfully registered in!")
      // window.location.href = "/auth"
    } else if (response.status == 400) {
      alert("Check if all fields are filled in.")
    } else if (response.status == 409) {
      alert("Email has already been used.")
    }
  }

  return (
    <form className={"auth-form"} onSubmit={handleSubmit}>
      <h1>Register</h1>
      <input 
        className={"input"} 
        type="text" 
        name="email"
        value={input.email}
        placeholder="Email@example.com" 
        onChange={handleChange}
      />
      {!isEmailValid && <span style={{ color: 'red', fontSize: "0.75rem" }}>Invalid email format</span>}  {/* Show error message */}
      
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
        type="submit"
      >
        Register
      </button>
    </form>
  )
}