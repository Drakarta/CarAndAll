import React, { useState } from 'react';

interface User {
    name: string;
    email: string;
    address: string;
    phoneNumber: string;
    role: string;
}

export default function GegevensInput(props: { user: User, edit: () => void }) {
    const [input, setInput] = useState({ naam: props.user.name, email: props.user.email, address: props.user.address, phoneNumber: props.user.phoneNumber });

    const handleChange = (e: { target: { name: string; value: string } }) => {
        setInput({ ...input, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/account/updateuser`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ naam: input.naam, email: input.email, adres: input.address, telefoonNummer: input.phoneNumber }),
            credentials: 'include',
        });

        if (response.status === 200) {
            alert("Successfully updated user information!");
            window.location.reload();
        } else if (response.status === 400) {
            alert("Check if all fields are filled in.");
        } else if (response.status === 401) {
            alert("Wrong credentials, please try again.");
        }
    };

    return (
        <form className="userinfo-form" onSubmit={handleSubmit}>
            <p>Name:</p>
            <input 
                type="text" 
                name="naam"
                value={input.naam} 
                placeholder="Naam"
                onChange={handleChange}
            />
            <p>Email:</p>
            <input 
                type="text" 
                name="email"
                value={input.email} 
                placeholder="Email@example.com"
                onChange={handleChange}
            />
            <p>Role:</p>
            <p className="role">{props.user.role}</p>
            <p>Address:</p>
            <input 
                type="text" 
                name="address"
                value={input.address} 
                placeholder="Address"
                onChange={handleChange}
            />
            <p>Phone Number:</p>
            <input 
                type="text" 
                name="phoneNumber"
                value={input.phoneNumber} 
                placeholder="1234567890"
                onChange={handleChange}
            />
            <div className="userinfo-buttons">
                <button type="button" onClick={props.edit}>Cancel</button>
                <button type="submit">Save</button>
            </div>
        </form>
    );
}