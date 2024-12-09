import { useEffect, useState } from "react"
import bcrypt from "bcryptjs"
import "../styles/loginRegisterForm.css"

export default function RegisterForm() {
  const [input, setInput] = useState({ email: "", password: "", repeatPassword: "" })
  const [isEmailValid, setIsEmailValid] = useState(true)

  const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/

  const handleChange = (e: { target: { name: any; value: any } }) => {
    setInput({ ...input, [e.target.name]: e.target.value })

    // Validate the email format on every change
    if (e.target.name === "email") {
      setIsEmailValid(emailRegex.test(e.target.value))
    }
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
    if (input.email.length < 4) {
      alert("Email must be at least 4 characters long")
      return
    }
    if (!isEmailValid) {
      alert("Please enter a valid email address.")
      return
    }

    const password = bcrypt.hashSync(input.password, import.meta.env.SALT)
    const response = await fetch(`${import.meta.env.VITE_REACT_APP_EMAIL_API_URL}/register`, {
      method: 'POST',
      headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${import.meta.env.VITE_REACT_APP_EMAIL_API_KEY}`
      },
      body: JSON.stringify({ email: input.email, password })
    })

    

    // window.location.href = "/auth"
  }
  
  useEffect(() => {
    const res =  fetch(`${import.meta.env.VITE_REACT_APP_API_URL}account/register`, {
      method: 'POST',
      headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${import.meta.env.VITE_REACT_APP_API_KEY}`
      },
      body: JSON.stringify({ Email: "test@test.nl", Password: "test" })
    })
    console.log(res)
  })
  

  return (
    <form className={"auth-form"}>
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
        onClick={handleSubmit}
      >
        Register
      </button>
    </form>
  )
}
