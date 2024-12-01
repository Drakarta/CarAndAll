import React, { useState } from 'react';
import axios from 'axios';

const EmailAdd: React.FC<{ setEmails: React.Dispatch<React.SetStateAction<string[]>> }> = ({ setEmails }) => {
    const [newEmail, setNewEmail] = useState<string>('');

    const addEmail = async () => {
        if (!newEmail) return;
        try {
            const response = await axios.post<string[]>('http://localhost:5016/api/email/emails/add', { Email: newEmail });
            setEmails(response.data);
            setNewEmail('');
        } catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error adding email:', error.message);
            } else {
                console.error('Error adding email:', error);
            }
        }
    };

    return (
        <div className="add-email">
            <h2>Add Email</h2>
            <input
                type="email"
                placeholder="Enter new email"
                value={newEmail}
                onChange={(e) => setNewEmail(e.target.value)}
            />
            <button onClick={addEmail}>Add</button>
        </div>
    );
};

export default EmailAdd;
