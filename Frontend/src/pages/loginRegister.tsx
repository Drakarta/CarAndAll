import { useState } from "react";

import LoginForm from "../components/loginForm";
import RegisterForm from "../components/registerForm";

import { useTokenStore } from "../stores";

export default function LoginRegister() {
  // State to determine if the user is registering or logging in
  const [register, setRegister] = useState(false);
  // State to determine if the user is a business or not
  const [bussiness, setBussiness] = useState(false);
  const token = useTokenStore((state) => state.token);
  // If the user is already logged in, redirect to the homepage
  if (token) {
    window.location.href = "/";
  }

  return (
    <>
      <div className={"page-center"}>
        <div className={"auth-container"}>
          {/** If the user is registering, show the register form, otherwise show the login form */}
          {register ? <RegisterForm bussiness={bussiness} /> : <LoginForm />}
          <button className={"button"} onClick={() => setRegister(!register)}>
            {register ? "Already have an account?" : "Don't have an account?"}
          </button>
          {/** If the user is registering, show the button to switch between business and private renter */}
          <button className={"button"} onClick={() => {setBussiness(!bussiness), setRegister(true)}}>
            {bussiness ? "Register as a private renter" : "Register as a business"}
          </button>
        </div>
      </div>
    </>
  )
}
