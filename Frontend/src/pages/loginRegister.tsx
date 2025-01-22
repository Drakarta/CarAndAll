import { useState } from "react";

import LoginForm from "../components/loginForm";
import RegisterForm from "../components/registerForm";

import { useTokenStore } from "../stores";

export default function LoginRegister() {
  const [register, setRegister] = useState(false);
  const token = useTokenStore((state) => state.token);
  if (token) {
    window.location.href = "/";
  }

  return (
    <>
      <div className={"page-center"}>
        <div className={"auth-container"}>
          {register ? <RegisterForm /> : <LoginForm />}
          <button className={"button"} onClick={() => setRegister(!register)}>
            {register ? "Already have an account?" : "Don't have an account?"}
          </button>
        </div>
      </div>
    </>
  )
}
