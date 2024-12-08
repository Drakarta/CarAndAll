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
          <NavLink to="/" className={"navBarLink"}>
            Link1
          </NavLink>
          <NavLink to="/" className={"navBarLink"}>
            Link2
          </NavLink>
        </div>
        <img className="navBarImage" src={Logo} alt="CarAndAll logo"></img>
        <button className="button login-button">Login</button>
      </nav>
      <Outlet />
    </>
  );
}
