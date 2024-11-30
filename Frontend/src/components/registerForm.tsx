import "../styles/loginRegisterForm.css"

export default function RegisterForm() {
  return (
    <form className="auth-form">
      <input type="text" placeholder="Username" />
      <input type="password" placeholder="Password" />
      <input type="password" placeholder="Repeat password" />
      <button className="button">Register</button>
    </form>
  )
}