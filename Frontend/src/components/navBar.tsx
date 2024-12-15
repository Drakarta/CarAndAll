import { NavLink, Outlet } from "react-router-dom";
import "../styles/navBar.css";
import Logo from "../assets/CarAndAllLogo.svg";
import { useTokenStore } from "../stores";
import LogOutButton from "./logOutButton";

export default function NavBar() {
  const token = useTokenStore((state) => state.token)
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
          <NavLink to="/" className={"navBarLink"}>
            Link2
          </NavLink>
        </div>
        <img className="navBarImage" src={Logo} alt="CarAndAll logo"></img>
        {token ? (
          <LogOutButton />
        ) : (
          <NavLink to="/auth" className="login-button">
            Login
          </NavLink>
        )}
      </nav>
      <Outlet />
    </>
  );
}