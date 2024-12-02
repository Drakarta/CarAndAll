import React, { useState, useEffect } from 'react';
import axios from 'axios';
import '../../styles/EmailManager.css';
import EmailList from '../../components/emailList';
import EmailAdd from '../../components/emailAdd';
import EmailRemove from '../../components/emailRemove';

const underOveriew: React.FC = () => {
    const [emails, setEmails] = useState<string[]>([]);

    useEffect(() => {
        fetchEmails();
    }, []);

    const fetchEmails = async () => {
        try {
            const response = await axios.get<string[]>('http://localhost:5016/api/email/emails');
            setEmails(response.data);
        } catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching emails:', error.message);
            } else {
                console.error('Error fetching emails:', error);
            }
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

export default underOveriew;
