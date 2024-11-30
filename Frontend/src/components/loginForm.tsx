import "../styles/loginRegisterForm.css";

export default function LoginForm() {
  return (
    <div>
      <form className="auth-form">
        <input type="text" placeholder="Username" />
        <input type="password" placeholder="Password" />
        <button>Login</button>
      </form>
    </div>
  )
}
