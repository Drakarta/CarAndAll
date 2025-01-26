import { useState } from 'react';

interface Account {
    id: string;
    naam: string;
    email: string;
    adres: string;
    telefoonNummer: string;
}

export default function EditBOAccount(props: { account: Account, edit: () => void }) {
    const [input, setInput] = useState({ naam: props.account.naam, email: props.account.email, adres: props.account.adres, telefoonNummer: props.account.telefoonNummer });

    const handleChange = (e: { target: { name: string; value: string } }) => {
        setInput({ ...input, [e.target.name]: e.target.value });

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        if (e.target.name === "email" && !emailRegex.test(e.target.value)) {
            alert("Invalid email format")
        }
    };

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/account/updatebackofficeaccount/${props.account.id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ naam: input.naam, email: input.email, adres: input.adres, telefoonNummer: input.telefoonNummer }),
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
                <h1>Edit Backoffice Account</h1>
                <div>
                    <form>
                        <div className='editBOAccountFormInput'>
                            <label htmlFor='naam'>Naam</label>
                            <input 
                                type='text' 
                                id='naam' 
                                name='naam' 
                                value={input.naam} 
                                onChange={handleChange}/>
                        </div>
                        <div className='editBOAccountFormInput'>
                            <label htmlFor='email'>Email</label>
                            <input 
                                type='text' 
                                id='email' 
                                name='email' 
                                value={input.email} 
                                onChange={handleChange} />
                        </div>
                        <div className='editBOAccountFormInput'>
                            <label htmlFor='adres'>Adres</label>
                            <input 
                                type='text' 
                                id='adres' 
                                name='adres' 
                                value={input.adres}
                                onChange={handleChange} />
                        </div>
                        <div className='editBOAccountFormInput'>
                            <label htmlFor='telefoonNummer'>Telefoonnummer</label>
                            <input 
                                type='text' 
                                id='telefoonNummer' 
                                name='telefoonNummer'
                                value={input.telefoonNummer}
                                onChange={handleChange} />
                        </div>
                        <div className='editBOAccountFormButtons'>
                            <button type='submit' className='button' onClick={handleSubmit} >Save</button>
                            <button type='button' className='button' onClick={props.edit}>Cancel</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    )
}
