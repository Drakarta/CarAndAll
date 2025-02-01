export default function DeleteBOAccount(props: { accountId: string }) {
    // Function to handle the submit of the form
    const handleSubmit = async (event: any) => {
        event.preventDefault()
        const result = window.confirm("Are you sure you want to delete this account?")
        if (!result) {
            return
        }
        const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/account/deletebackofficeaccount/${props.accountId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'include',
        })
        if (response.status == 200) {
            alert("Successfully deleted account!")
        } else if (response.status == 400) {
            alert("Failed to delete account!")
        }
        // Reload the page to show the updated list of accounts
        window.location.reload()
    }
    return (
        <button onClick={handleSubmit}>Delete</button>
    )
}
