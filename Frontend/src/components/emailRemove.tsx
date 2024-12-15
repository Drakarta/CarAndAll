import React, { useState } from 'react';
import { useTokenStore } from "../stores";

interface EmailRemoveProps {
    setEmails: React.Dispatch<React.SetStateAction<string[]>>;
}

const EmailRemove: React.FC<EmailRemoveProps> = ({ setEmails }) => {
    const [email, setEmail] = useState('');
    const token = useTokenStore((state) => state.token);
    const role = useTokenStore((state) => state.role);
    const handleRemoveEmail = async () => {
        try {
            const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/${role}Email/removeUserFromCompany`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({ email }),
            });
            if (response.ok) {
                setEmails(prevEmails => prevEmails.filter(e => e !== email));
                setEmail('');
            } else {
                console.error('Error removing email:', response.statusText);
            }
        } catch (error) {
            console.error('Error removing email:', error);
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
            <button onClick={handleRemoveEmail}>Remove Email</button>
        </div>
    );
};

export default EmailRemove;
