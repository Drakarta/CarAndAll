import { useState } from "react"

export default function CreateBOAccount(props: {create: () => void}) {
    const [input, setInput] = useState({ naam: "", email: "", password: "", adres: "", telefoonNummer: "" })
    const [isEmailValid, setIsEmailValid] = useState(true)
    const [isPasswordValid, setIsPasswordValid] = useState(true)

    const handleChange = (e: { target: { name: any; value: any } }) => {
        setInput({ ...input, [e.target.name]: e.target.value })

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        if (e.target.name === "email" && !emailRegex.test(e.target.value)) {
            setIsEmailValid(false)
        } else if (e.target.name === "email" && emailRegex.test(e.target.value)) {
            setIsEmailValid(true)
        }

        if (e.target.name === "password" && e.target.value === "") {
            setIsPasswordValid(false)
        } else if (e.target.name === "password" && e.target.value !== "") {
            setIsPasswordValid(true)
        }
    }

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        if (!isEmailValid) {
            alert("Please enter a valid email address.");
            return
        }

        if (!isPasswordValid) {
            alert("Please enter a password.");
            return
        }
        const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/account/createbackofficeaccount`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ naam: input.naam, email: input.email, password: input.password, address: input.adres, phoneNumber: input.telefoonNummer }),
            credentials: 'include',
        });

        if (response.status === 200) {
            alert("Successfully updated backoffice account!");
            window.location.reload();
        } else if (response.status === 400) {
            alert("Check if all fields are filled in.");
        } else if (response.status === 401) {
            alert("Wrong credentials, please try again.");
        }
    };

  return (
    <div className='page-center editBOAccount'>
        <div className='editBOAccountContainer'>
            <h1>Create Backoffice Account</h1>
            <div>
                <form>
                    <div className='editBOAccountFormInput'>
                        <label htmlFor='naam'>Naam</label>
                        <input 
                            type='text' 
                            id='naam' 
                            name='naam' 
                            value={input.naam} 
                            onChange={handleChange}
                            />
                    </div>
                    <div className='editBOAccountFormInput'>
                        <label htmlFor='email'>Email</label>
                        <input 
                            type='text' 
                            id='email' 
                            name='email' 
                            value={input.email} 
                            onChange={handleChange} 
                            required
                            />
                    </div>
                    <div className='editBOAccountFormInput'>
                        <label htmlFor='password'>Password</label>
                        <input 
                            type='password' 
                            id='password' 
                            name='password' 
                            value={input.password} 
                            onChange={handleChange} 
                            required
                            />
                    </div>
                    <div className='editBOAccountFormInput'>
                        <label htmlFor='adres'>Adres</label>
                        <input 
                            type='text' 
                            id='adres' 
                            name='adres' 
                            value={input.adres}
                            onChange={handleChange} 
                            />
                    </div>
                    <div className='editBOAccountFormInput'>
                        <label htmlFor='telefoonNummer'>Telefoonnummer</label>
                        <input 
                            type='text' 
                            id='telefoonNummer' 
                            name='telefoonNummer'
                            value={input.telefoonNummer}
                            onChange={handleChange} 
                            />
                    </div>
                    <div className='editBOAccountFormButtons'>
                        <button type='submit' className='button' onClick={handleSubmit} >Create</button>
                        <button type='button' className='button' onClick={props.create}>Cancel</button>
                    </div>
                </form>
            </div>

        </div>
    </div>
            
  )
}
