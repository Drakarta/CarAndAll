import { useState } from "react";

import LoginForm from "../components/loginForm";
import RegisterForm from "../components/registerForm";

import { useTokenStore } from "../stores";

export default function LoginRegister() {
  const [register, setRegister] = useState(false);
  const [bussiness, setBussiness] = useState(false);
  const token = useTokenStore((state) => state.token);
  if (token) {
    window.location.href = "/";
  }

  return (
    <>
      <div className={"page-center"}>
        <div className={"auth-container"}>
          {register ? <RegisterForm bussiness={bussiness} /> : <LoginForm />}
          <button className={"button"} onClick={() => setRegister(!register)}>
            {register ? "Already have an account?" : "Don't have an account?"}
          </button>
          <button className={"button"} onClick={() => {setBussiness(!bussiness), setRegister(true)}}>
            {bussiness ? "Register as a private renter" : "Register as a business"}
          </button>
        </div>
      </div>
    </>
  )
}
