import { NavLink, Outlet } from "react-router-dom";
import "../styles/navBar.css";
import Logo from "../assets/CarAndAllLogo.svg";
import { useTokenStore } from "../stores";
import LogOutButton from "./logOutButton";

const BaseNavBar: React.FC = () => (
  <>
    <NavLink to="/" className="navBarLink">
      Home
    </NavLink>
    <NavLink to="/profile" className="navBarLink">
      Profile
    </NavLink>
    <NavLink to ="/verhuur-aanvraag-status" className="navBarLink">
      Verhuur Aanvraag Status
    </NavLink>
  </>
);

const AdminNavBar: React.FC = () => (
  <>
    <BaseNavBar />
    <NavLink to="/voertuigenOverview" className="navBarLink">
      Voertuigen
    </NavLink>
    <NavLink to="/email-manager" className="navBarLink">
      Email Manager
    </NavLink>
    <NavLink to="/abonnementen" className="navBarLink">
      Abonnementen
    </NavLink>
  </>
);

const BeheerderNavBar : React.FC = () => (
  <>
          <BaseNavBar />
          <NavLink to="/voertuigenOverview" className="navBarLink">
            Voertuigen
          </NavLink>
          <NavLink to="/email-manager" className="navBarLink">
            Email Manager
          </NavLink>
          <NavLink to="/abonnementen" className="navBarLink">
            Abonnementen
          </NavLink>
        </>
)

const BackOfficeNavBar : React.FC = () => (
  <>
          <BaseNavBar />
          <NavLink to="/voertuigenOverview" className="navBarLink">
            Voertuigen
          </NavLink>
        </>
)

const FrontOfficeNavBar: React.FC = () => (
  <>
    <BaseNavBar />
    <NavLink to="/frontofficedashboard" className="navBarLink">
      front office Dashboard
    </NavLink>
  </>
);
const RoleNavBar: React.FC<{ role: string }> = ({ role }) => {
  switch (role) {
    case "Admin":
      return <AdminNavBar />;
    case "Wagenparkbeheerder":
    case "Zakelijkeklant":
      return <BeheerderNavBar />;
      case "Backofficemedewerker":
        return <BackOfficeNavBar />;
    case "Frontofficemedewerker":
      return <FrontOfficeNavBar />;
    default:
      return <BaseNavBar />;
  }
};

export default function NavBar() {
  const token = useTokenStore((state) => state.token);
  const role = useTokenStore((state) => state.role) || "";

  return (
    <>
      <nav className="nav">
        <div className="navLinkContainer">
          <RoleNavBar role={role} />
        </div>
        <img className="navBarImage" src={Logo} alt="CarAndAll logo" />
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