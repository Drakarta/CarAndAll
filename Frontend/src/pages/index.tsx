import { useEffect, useState } from "react";

// Define the user object type
interface User {
  id: string;
  email: string;
  name: string;
  address: string;
  phoneNumber: string;
  role: string;
}

export default function Index() {
  const [user, setUser] = useState<User | null>(null); // Initialize as null for "no data" state
  const [loading, setLoading] = useState<boolean>(true); // Track loading state
  const [error, setError] = useState<string | null>(null); // Track errors

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
      {loading ? (
        <p>Loading...</p>
      ) : error ? (
        <p style={{ color: "red" }}>{error}</p>
      ) : user ? (
        <div>
          <h1>Welcome, {user.name}</h1>
          <p>Email: {user.email}</p>
          <p>Role: {user.role}</p>
          <p>Address: {user.address}</p>
          <p>Phone: {user.phoneNumber}</p>
        </div>
      ) : (
        <p>No user data available.</p>
      )}
    </div>
  );
}