import React, { useState } from 'react';
import './emailButton.css';
interface EmailRemoveProps {
    readonly setEmails: React.Dispatch<React.SetStateAction<string[]>>;
}

//Verwijdert een account van een bedrijf
export default function EmailRemove({ setEmails }: EmailRemoveProps) {
    const [email, setEmail] = useState('');
    const [error, setError] = useState('');

    const handleRemoveEmail = async () => {
        try {
            const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/Wagenparkbeheerder/removeUserFromCompany`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify({ email }),
            });
            if (response.ok) {
                setEmails((Emails) => Emails.filter(e => e !== email.toLowerCase()));
                setEmail('');
                setError('');
                window.location.reload();
            } else {
                setError(`Error removing email: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error removing email:', error);
            setError('Error removing email');
        }
    };

    return (
        <div>
            <input
                id="emailRemoveLabel"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="Enter email to remove"
            />
            <button 
                onClick={handleRemoveEmail} 
                disabled={!email} 
                className={!email ? 'disabled' : ''}
            >
                Remove Email
            </button>
            {error && <p>{error}</p>}
        </div>
    );
};