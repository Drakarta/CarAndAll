import "../styles/loginRegisterForm.css"

export default function RegisterForm() {
  return (
    <form className="auth-form">
      <input className="input" type="text" placeholder="Username" />
      <input className="input" type="password" placeholder="Password" />
      <input className="input" type="password" placeholder="Repeat password" />
      <button className="button">Register</button>
    </form>
  )
}