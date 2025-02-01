import { useTokenStore } from '../stores';

// Function to log out the user
export default function LogOutButton() {
  const { deleteToken } = useTokenStore();
  // Function to log out the user
  const LogOut = async () => {
    try {
      await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/account/logout`, {
        method: 'POST',
        credentials: 'include',
    }).then(() => {
          deleteToken();
          // Reload the page to show the updated list of accounts
          window.location.reload();
        })
    } catch (error) {
      console.error('An error occurred during logout', error);
    }
  }

  return (
    <button onClick={LogOut} className={"login-button"}>Log out</button>
  )
}