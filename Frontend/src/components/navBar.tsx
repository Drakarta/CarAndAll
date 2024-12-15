import { NavLink, Outlet } from "react-router-dom";
import "../styles/navBar.css";
import Logo from "../assets/CarAndAllLogo.svg";

export default function NavBar() {
  return (
    <>
      <nav className="nav">
        <div className="navLinkContainer">
          <NavLink to="/" className={"navBarLink"}>
            Home
          </NavLink>
          <NavLink to="/voertuigenOverview" className={"navBarLink"}>
            Voertuigen
          </NavLink>
          <NavLink to="/email-manager" className={"navBarLink"}>
            Email manager
          </NavLink>
          <NavLink to="/abonnementen" className={"navBarLink"}>
            Abonnementen
          </NavLink>
        </div>
        <img className="navBarImage" src={Logo} alt="CarAndAll logo"></img>
        <NavLink to="/auth" className={"login-button"}>Login</NavLink>
      </nav>
      <Outlet />
    </>
  );
}
