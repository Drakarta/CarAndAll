import { useTokenStore } from '../stores';

export default function LogOutButton() {
  const { deleteToken } = useTokenStore();

  const LogOut = async () => {
    try {
      await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/account/logout`, {
        method: 'POST',
        credentials: 'include',
    }).then(() => {
          deleteToken();
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