import "../styles/loginRegisterForm.css";

export default function LoginForm() {
  return (
    <div>
      <form className="auth-form">
        <input className="input" type="text" placeholder="Username" />
        <input className="input" type="password" placeholder="Password" />
        <button className="button">Login</button>
      </form>
    </div>
  )
}
