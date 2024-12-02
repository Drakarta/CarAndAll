import React, { useState } from 'react';
import axios from 'axios';

interface EmailRemoveProps {
    setEmails: React.Dispatch<React.SetStateAction<string[]>>;
}

const EmailRemove: React.FC<EmailRemoveProps> = ({ setEmails }) => {
    const [email, setEmail] = useState('');

    const handleRemoveEmail = async () => {
        try {
            const response = await axios.post('http://localhost:5016/api/email/removeUserFromCompany', { email });
            if (response.status === 200) {
                setEmails(prevEmails => prevEmails.filter(e => e !== email));
                setEmail('');
            }
        } catch (error) {
            console.error('Error removing email:', error);
        }
    };

    return (
        <div>
            <input
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
