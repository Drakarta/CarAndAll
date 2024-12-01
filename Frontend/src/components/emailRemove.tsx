import React, { useState } from 'react';
import axios from 'axios';

const EmailRemove: React.FC<{ setEmails: React.Dispatch<React.SetStateAction<string[]>> }> = ({ setEmails }) => {
    const [emailToRemove, setEmailToRemove] = useState<string>('');

    const removeEmail = async () => {
        if (!emailToRemove) return;
        try {
            const response = await axios.delete<string[]>(`http://localhost:5016/api/email/emails/${emailToRemove}`);
            setEmails(response.data);
            setEmailToRemove('');
        } catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error removing email:', error.message);
            } else {
                console.error('Error removing email:', error);
            }
        }
    };

    return (
        <div className="remove-email">
            <h2>Remove Email</h2>
            <input
                type="email"
                placeholder="Enter email to remove"
                value={emailToRemove}
                onChange={(e) => setEmailToRemove(e.target.value)}
            />
            <button onClick={removeEmail}>Remove</button>
        </div>
    );
};

export default EmailRemove;
