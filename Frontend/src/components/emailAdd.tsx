import React, { useState } from 'react';
import axios from 'axios';

interface EmailAddProps {
    setEmails: React.Dispatch<React.SetStateAction<string[]>>;
}

const EmailAdd: React.FC<EmailAddProps> = ({ setEmails }) => {
    const [email, setEmail] = useState('');

    const handleAddEmail = async () => {
        try {
            const response = await axios.post('http://localhost:5016/api/email/addUserToCompany', { email });
            if (response.status === 200) {
                setEmails(prevEmails => [...prevEmails, email]);
                setEmail('');
            }
        } catch (error) {
            console.error('Error adding email:', error);
        }
    };

    return (
        <div>
            <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="Enter email"
            />
            <button onClick={handleAddEmail}>Add Email</button>
        </div>
    );
};

export default EmailAdd;
