export default function DeleteBOAccount(props: { accountId: string }) {
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
        window.location.reload()
    }
    return (
        <button onClick={handleSubmit}>Delete</button>
    )
}
