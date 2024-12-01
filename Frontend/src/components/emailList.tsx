import React from 'react';

const EmailList: React.FC<{ emails: string[] }> = ({ emails }) => {
    return (
        <div className="email-list">
            <h2>Current Emails</h2>
            <ul>
                {emails.map((email, index) => (
                    <li key={index}>{email}</li>
                ))}
            </ul>
        </div>
    );
};

export default EmailList;