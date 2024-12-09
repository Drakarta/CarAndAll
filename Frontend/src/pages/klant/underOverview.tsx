import React, { useState, useEffect } from 'react';
import '../../styles/EmailManager.css';
import EmailList from '../../components/emailList';
import EmailAdd from '../../components/emailAdd';
import EmailRemove from '../../components/emailRemove';

const UnderOverview: React.FC = () => {
    const [emails, setEmails] = useState<string[]>([]);

    useEffect(() => {
        fetchEmails();
    }, []);

    const fetchEmails = async () => {
        try {
            const response = await fetch(`${import.meta.env.VITE_REACT_APP_EMAIL_API_URL}/emails`, {
                headers: {
                    'Authorization': `Bearer ${import.meta.env.VITE_REACT_APP_API_KEY}`
                }
            });
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const data = await response.json();
            setEmails(data);
        } catch (error) {
            console.error('Error fetching emails:', error);
        }
    };

    return (
        <div className="email-manager">
            <div className="header">Email Manager</div>
            <div className="content">
                <div className="email-list">
                    <EmailList emails={emails} />
                </div>
                <div className="email-actions">
                    <EmailAdd setEmails={setEmails} />
                    <EmailRemove setEmails={setEmails} />
                </div>
            </div>
        </div>
    );
};

export default UnderOverview;
