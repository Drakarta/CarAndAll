import { useState } from "react";

import { useTokenStore } from "../stores";

import "../styles/loginRegisterForm.css";

export default function LoginForm() {
  const setToken = useTokenStore((state) => state.setToken);
  const [input, setInput] = useState({ username: "", password: "" });
  const handleChange = (e: { target: { name: any; value: any } }) => {
    setInput({ ...input, [e.target.name]: e.target.value });
  };

  const handleSubmit = async () => {
    setToken("Token");

    window.location.href = "/";
  }



  return (
    <div>
      <form className="auth-form">
        <h1>Login</h1>
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
