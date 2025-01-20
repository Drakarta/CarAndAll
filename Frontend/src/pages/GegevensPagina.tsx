import { useEffect, useState } from "react";
import GegevensInput from "../components/gegevensInput";
import GegevensShow from "../components/gegevensShow";

import "../styles/gegevens.css";
import { useLocation } from "react-router-dom";

// Define the user object type
interface User {
  id: string;
  email: string;
  name: string;
  address: string;
  phoneNumber: string;
  role: string;
}


export default function GegevensPagina() {
    const location = useLocation();
    const queryParams = new URLSearchParams(location.search);
    const newuser = queryParams.get('newuser');
    const [user, setUser] = useState<User | null>(null); // Initialize as null for "no data" state
    const [loading, setLoading] = useState<boolean>(true); // Track loading state
    const [error, setError] = useState<string | null>(null); // Track errors
    const [edit, setEdit] = useState<boolean>(newuser === "true" ? false : true);
  
    useEffect(() => {
      const fetchUserData = async () => {
        try {
          const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/account/getuserbyid`, {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
            },
            credentials: 'include',
          });
  
          if (response.status === 200) {
            const data = await response.json();
            setUser({
              id: data.userId,
              email: data.email,
              name: data.name,
              address: data.address,
              phoneNumber: data.phoneNumber,
              role: data.role,
            });
            setError(null); // Clear any previous errors
          } else {
            setError(`Failed to fetch user data. Status: ${response.status}`);
          }
        } catch (err) {
          setError("An error occurred while fetching user data.");
          console.error(err);
        } finally {
          setLoading(false); // Stop loading regardless of success or failure
        }
      };
    fetchUserData();
  }, []);
  return (
    <div>
      <h1>GegevensPagina</h1>
      {loading ? (
        <p>Loading...</p>
      ) : error ? (
        <p style={{ color: "red" }}>{error}</p>
      ) : user ? (
        <>
        <div className={"userinfo"}> 
          {edit ? <GegevensShow user={user} edit={() => setEdit(!edit)} /> : <GegevensInput user={user} edit={() => setEdit(!edit)}/>}
        </div>
        </>
      ) : (<p>No user data available.</p>)
      }
    </div>
  )
}