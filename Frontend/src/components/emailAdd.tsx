import React, { useState } from 'react';

interface EmailAddProps {
    setEmails: React.Dispatch<React.SetStateAction<string[]>>;
}

const EmailAdd: React.FC<EmailAddProps> = ({ setEmails }) => {
    const [email, setEmail] = useState('');
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [responseMessage, setResponseMessage] = useState<string | null>(null);

    const handleAddEmail = async () => {
        try {
            const response = await fetch(`${import.meta.env.VITE_REACT_APP_EMAIL_API_URL}/addUserToCompany`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${import.meta.env.VITE_REACT_APP_API_KEY}`
                },
                body: JSON.stringify({ email }),
            });

            let responseData;
            const contentType = response.headers.get('Content-Type');
            if (contentType && contentType.includes('application/json')) {
                responseData = await response.json();
            } else {
                responseData = { message: await response.text() };
            }

            if (response.ok) {
                setEmails(prevEmails => [...prevEmails, email]);
                setEmail('');
                setErrorMessage(null);
                setResponseMessage(responseData.message);
            } else {
                setResponseMessage(null);
                if (responseData.status === 404 && responseData.message === 'NonExEmail') {
                    setErrorMessage('Email not found in database');
                } else if (responseData.statusCode === 400 && responseData.message === 'FalseFormatEmail') {
                    setErrorMessage('This is not the correct format of an email');
                } else if (responseData.statusCode === 500 && responseData.message === 'MaxNumber') {
                    setErrorMessage('You have reached the maximum amount of users allowed for your subscription.');
                } else if (responseData.statusCode === 400 && responseData.message === 'FalseDomein') {
                    setErrorMessage('This email is not allowed to be added to the company.');
                }  else {
                    setErrorMessage('An unexpected error occurred. Please try again.');
                }
            }
        } catch (error) {
            console.error('Error adding email:', error);
            setErrorMessage('An unexpected error occurred. Please try again.');
            setResponseMessage(null);
        }
    };

    const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
        if (event.key === 'Enter') {
            handleAddEmail();
        }
    };

    return (
        <div>
            <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="Enter email"
                onKeyDown={handleKeyDown}
            />
            <button onClick={handleAddEmail}>Add Email</button>
            {errorMessage && <p>{errorMessage}</p>}
            {responseMessage && <p>{responseMessage}</p>}
        </div>
    );
};

export default EmailAdd;
