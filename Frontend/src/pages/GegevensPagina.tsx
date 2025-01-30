import { useEffect, useState } from "react";
import GegevensInput from "../components/gegevensInput";
import GegevensShow from "../components/gegevensShow";

import "../styles/gegevens.css";
import { useLocation } from "react-router-dom";

// Interface for the user object
interface User {
  id: string;
  email: string;
  name: string;
  address: string;
  phoneNumber: string;
  role: string;
}


export default function GegevensPagina() {
    // Get the query parameters from the URL
    const location = useLocation();
    const queryParams = new URLSearchParams(location.search);
    const newuser = queryParams.get('newuser');

    // State to store the user object
    const [user, setUser] = useState<User | null>(null);
    // State to determine if the page is still loading
    const [loading, setLoading] = useState<boolean>(true);
    // State to store any errors that occur during fetching
    const [error, setError] = useState<string | null>(null);
    // State to determine if the user is in edit mode
    const [edit, setEdit] = useState<boolean>(newuser === "true" ? false : true);

    // Fetch the user data from the API
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

          if (response.status === 405) {
            window.location.href = "/404";
          } 
  
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
          {/* If the user is in edit mode show the GegevensInput component, otherwise show the GegevensShow component */}
          {edit ? <GegevensShow user={user} edit={() => setEdit(!edit)} /> : <GegevensInput user={user} edit={() => setEdit(!edit)}/>}
        </div>
        </>
      ) : (<p>No user data available.</p>)
      }
    </div>
  )
}