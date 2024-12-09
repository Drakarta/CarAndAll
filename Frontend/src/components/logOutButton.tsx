import { useTokenStore } from '../stores'

export default function LogOutButton() {
  const deleteToken = useTokenStore(state => state.deleteToken)
  function logOut() {
    deleteToken()
    window.location.href = "/"
  }

  return (
    <button onClick={() => logOut} className={"button"}>Log out</button>
  )
}
