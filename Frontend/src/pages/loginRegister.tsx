import { useState } from "react";

import LoginForm from "../components/loginForm";
import RegisterForm from "../components/registerForm";

export default function LoginRegister() {
    const [register, setRegister] = useState(false);
  return (
    <>
      <div className="page-center">
        <div className="auth-container">
          {register ? <RegisterForm /> : <LoginForm />}
          <button onClick={() => setRegister(!register)}>
            {register ? "Already have an account?" : "Don't have an account?"}
          </button>
        </div>
      </div>
    </>
  )
}
