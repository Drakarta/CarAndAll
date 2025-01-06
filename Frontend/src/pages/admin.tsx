import { useEffect, useState } from "react";
import "../styles/admin.css";

// Define a User type for simplicity
interface User {
  id: string;
  email: string;
  name: string; // Added name field
  role: string;
  address: string;
  phoneNumber: string;
}

export default function AdminPanel() {
  const [users, setUsers] = useState<User[]>([]);
  const [newUserEmail, setNewUserEmail] = useState<string>("");
  const [newUserName, setNewUserName] = useState<string>(""); // Track new user name
  const [newUserRole, setNewUserRole] = useState<string>("");
  const [newUserAddress, setNewUserAddress] = useState<string>("");
  const [newUserPhone, setNewUserPhone] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [editingUserId, setEditingUserId] = useState<string | null>(null); // Track editing user ID
  const [editedName, setEditedName] = useState<string>(""); // Track edited name
  const [editedEmail, setEditedEmail] = useState<string>(""); // Track edited email
  const [editedRole, setEditedRole] = useState<string>(""); // Track edited role

  // Fetch users from the API
  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/account/users`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
          },
          credentials: "include",
        });

        if (response.ok) {
          const data = await response.json();
          setUsers(data.users); // Assuming API returns a `users` array
        } else {
          setError(`Failed to fetch users. Status: ${response.status}`);
        }
      } catch (err) {
        console.error(err);
        setError("An error occurred while fetching users.");
      } finally {
        setLoading(false);
      }
    };

    fetchUsers();
  }, []);

  // Add a new user
  const addUser = async () => {
    if (!newUserEmail || !newUserName || !newUserRole || !newUserAddress || !newUserPhone) {
      setError("Please fill out all fields.");
      return;
    }
  
    try {
      const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/account/CreateUser`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({
          email: newUserEmail,
          password: "TemporaryPassword123!",
          role: newUserRole,
          address: newUserAddress,
          phoneNumber: newUserPhone,
          name: newUserName, // Ensure name is sent to the backend
        }),
      });
  
      if (response.ok) {
        const data = await response.json();
        setUsers([...users, {
          id: data.UserId,
          email: newUserEmail,
          name: newUserName,
          role: newUserRole,
          address: newUserAddress,
          phoneNumber: newUserPhone,
        }]);
        setNewUserEmail("");
        setNewUserName("");
        setNewUserRole("");
        setNewUserAddress("");
        setNewUserPhone("");
        setError(null);
      } else {
        const errorData = await response.json();
        setError(`Failed to add user. ${errorData.message || "Unknown error."}`);
      }
    } catch (err) {
      console.error(err);
      setError("An error occurred while adding the user.");
    }
  };

  const updateUser = async (id: string) => {
  if (!editedName || !editedEmail || !editedRole) {
    setError("Please provide name, email, and role.");
    return;
  }

  try {
    const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/account/users/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({
        name: editedName,
        email: editedEmail,
        role: editedRole,
      }),
    });

    if (response.ok) {
      const updatedData = await response.json();
      setUsers((prevUsers) =>
        prevUsers.map((user) =>
          user.id === id
            ? { ...user, name: updatedData.name, email: updatedData.email, role: updatedData.role }
            : user
        )
      );
      setEditingUserId(null);
      setError(null);
    } else {
      const errorData = await response.json();
      setError(`Failed to update user. ${errorData.message || "Unknown error."}`);
    }
  } catch (err) {
    console.error(err);
    setError("An error occurred while updating the user.");
  }
};

  

  // Remove a user
  const removeUser = async (id: string) => {
    try {
      const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/account/users/${id}`, {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
      });
  
      if (response.ok) {
        // Remove the user from the local state
        setUsers((prevUsers) => prevUsers.filter((user) => user.id !== id));
        setError(null);
      } else {
        const errorData = await response.json();
        setError(`Failed to remove user. ${errorData.message || "Unknown error."}`);
      }
    } catch (err) {
      console.error(err);
      setError("An error occurred while removing the user.");
    }
  };
  

  if (loading) {
    return <p>Loading...</p>;
  }

  if (error) {
    return <p style={{ color: "red" }}>{error}</p>;
  }

  return (
    <div style={{ padding: "20px" }}>
      <h1>Admin Panel</h1>

      {/* Add New User Section */}
      <div style={{ marginBottom: "20px" }}>
        <h2>Add New User</h2>
        <input
          type="email"
          placeholder="Enter user email"
          value={newUserEmail}
          onChange={(e) => setNewUserEmail(e.target.value)}
        />
        <input
          type="text"
          placeholder="Enter user name"
          value={newUserName}
          onChange={(e) => setNewUserName(e.target.value)}
        />
        <input
          type="text"
          placeholder="Enter user address"
          value={newUserAddress}
          onChange={(e) => setNewUserAddress(e.target.value)}
        />
        <input
          type="text"
          placeholder="Enter user phone number"
          value={newUserPhone}
          onChange={(e) => setNewUserPhone(e.target.value)}
        />
        <input
          type="text"
          placeholder="Enter user role"
          value={newUserRole}
          onChange={(e) => setNewUserRole(e.target.value)}
        />
        <button onClick={addUser}>Add User</button>
      </div>

      {/* User Management Section */}
      <div>
        <h2>Manage Users</h2>
        {users.length === 0 ? (
          <p>No users available.</p>
        ) : (
          <table style={{ borderCollapse: "collapse", width: "100%" }}>
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Email</th>
                <th>Role</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {users.map((user) => (
                <tr key={user.id}>
                  <td>{user.id}</td>
                  <td>
                    {editingUserId === user.id ? (
                      <input
                        type="text"
                        value={editedName}
                        onChange={(e) => setEditedName(e.target.value)}
                      />
                    ) : (
                      user.name
                    )}
                  </td>
                  <td>
                    {editingUserId === user.id ? (
                      <input
                        type="email"
                        value={editedEmail}
                        onChange={(e) => setEditedEmail(e.target.value)}
                      />
                    ) : (
                      user.email
                    )}
                  </td>
                  <td>
                    {editingUserId === user.id ? (
                      <input
                        type="text"
                        value={editedRole}
                        onChange={(e) => setEditedRole(e.target.value)}
                      />
                    ) : (
                      user.role
                    )}
                  </td>
                  <td>
                    {editingUserId === user.id ? (
                      <button onClick={() => updateUser(user.id)}>Save</button>
                    ) : (
                      <button
                        onClick={() => {
                          setEditingUserId(user.id);
                          setEditedName(user.name);
                          setEditedEmail(user.email);
                          setEditedRole(user.role);
                        }}
                      >
                        Edit
                      </button>
                    )}
                    <button onClick={() => removeUser(user.id)}>Remove</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}
