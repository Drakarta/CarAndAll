import { NavLink, Outlet } from "react-router-dom";
import Logo from "../assets/CarAndAllLogo.svg";
import { useTokenStore } from "../stores";
import LogOutButton from "./logOutButton";


import "../styles/navBar.css";


const BaseNavBar: React.FC = () => (
  <>
    <NavLink to="/" className="navBarLink">
      Home
    </NavLink>
    <NavLink to="/profile" className="navBarLink">
      Profile
    </NavLink>
  </>
);

const ParticuliereNavBar: React.FC = () => (
  <>
    <NavLink to="/" className="navBarLink">
      Home
    </NavLink>
    <NavLink to="/voertuigenOverview" className="navBarLink">
      Voertuigen
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
    <NavLink to="/WagenParkBeheerder" className="navBarLink">
            WagenParkBeheerder
    </NavLink>
    <NavLink to="/admin" className="NarBarLink">
    admin
    </NavLink>
  </>
);

const WagenParkNavBar : React.FC = () => (

  <>
          <BaseNavBar />
          <NavLink to="/WagenParkBeheerder" className="navBarLink">
            WagenParkBeheerder
          </NavLink>
          <NavLink to="/abonnementen" className="navBarLink">
            Abonnementen
          </NavLink>
        </>
);


const ZakelijkeKlantNavBar : React.FC = () => (

  <>
          <BaseNavBar />
          <NavLink to="/voertuigenOverview" className="navBarLink">
            Voertuigen
          </NavLink>
          <NavLink to ="/verhuur-aanvraag-status" className="navBarLink">
      Verhuur Aanvraag Status
    </NavLink>
        </>
);

const BackOfficeNavBar : React.FC = () => (
  <>
          <BaseNavBar />
          <NavLink to="/voertuigenOverview" className="navBarLink">
            Voertuigen
          </NavLink>
          <NavLink to="/backOfficeVerhuurAanvragen" className="navBarLink">
            Verhuur aanvragen
          </NavLink>
          <NavLink to="/backOfficeAbonnementAanvragen" className="navBarLink">
            Abonnement aanvragen
            </NavLink>
          <NavLink to="/BackOfficeAccounts" className="navBarLink">
            Backoffice Accounts
          </NavLink>
          <NavLink to="/privacy/edit" className="navBarLink">
            privacy edit
          </NavLink>
        </>
);

const FrontOfficeNavBar: React.FC = () => (
  <>
    <BaseNavBar />
    <NavLink to="/frontofficedashboard" className="navBarLink">
      front office Dashboard
    </NavLink>
  </>
);
const RoleNavBar: React.FC<{ role: string }> = ({ role }) =>  {
  switch (role) {
    case "Admin":
      return <AdminNavBar />;
    case "Wagenparkbeheerder":
      return <WagenParkNavBar />;
    case "Zakelijkeklant":
      return <ZakelijkeKlantNavBar />;
    case "Backofficemedewerker":
      return <BackOfficeNavBar />;
    case "Particuliere huurder":
      return <ParticuliereNavBar />;
    case "Frontofficemedewerker":
      return <FrontOfficeNavBar />;
    default:
      return <BaseNavBar />;
  }
};

export default function NavBar() {
  const token = useTokenStore((state) => state.token);
  const role = useTokenStore((state) => state.role) ?? "";

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