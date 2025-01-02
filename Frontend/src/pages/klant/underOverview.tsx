import React, { useState, useEffect } from 'react';
import '../../styles/EmailManager.css';
import EmailList from '../../components/emailList';
import EmailAdd from '../../components/emailAdd';
import EmailRemove from '../../components/emailRemove';
import { useTokenStore } from '../../stores';

const UnderOverview: React.FC = () => {
    const [emails, setEmails] = useState<string[]>([]);
    //const token = useTokenStore((state) => state.token);
    const role = useTokenStore((state) => state.role);

    useEffect(() => {
        fetchEmails();
    
    }, []);

    const fetchEmails = async () => {
        try {
            const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/${role}Email/emails`, {
                method: 'GET',
                credentials: 'include',
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
            <div className="header">
                Email Manager for {role === 'Wagenparkbeheerder' ? 'Wagenparkbeheerder' : 'Zakelijkeklant'}
            </div>
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
