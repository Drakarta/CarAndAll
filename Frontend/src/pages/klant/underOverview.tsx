import { useState, useEffect } from 'react';
import '../../styles/EmailManager.css';
import EmailList from '../../components/emailList';
import EmailAdd from '../../components/emailAdd';
import EmailRemove from '../../components/emailRemove';
import { useNavigate } from "react-router-dom"; 
import DisplayHistory from '../../components/displayHistory';
import { useAanvragenState } from '../../state/aanvragenState';

export default function UnderOverview() {
    const [emails, setEmails] = useState<string[]>([]);
    const { aanvragen, setAanvragen } = useAanvragenState();
    const navigate = useNavigate();

    useEffect(() => {
        fetchEmails();
    }, []);

    const fetchEmails = async () => {
        try {
            const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/Wagenparkbeheerder/emails`, {
                method: 'GET',
                credentials: 'include',
            });
            if (response.status === 404) {
                window.location.href = "/abonnementen";
            }
            if (response.status === 405) {
                window.location.href = "/404";
            }
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
                Wagenparkbeheerder panel
            </div>
            <div>
                <button id="abb" onClick={() => navigate('/abonnementen')}>Abonnement</button>
            </div>
            <div className="content">
                <div className="email-list">
                    <EmailList emails={emails} setAanvragen={setAanvragen}/>
                </div>
                <div className="email-actions">
                    <EmailAdd setEmails={setEmails} />
                    <EmailRemove setEmails={setEmails} />
                </div>
            </div>
            <div>
                {aanvragen !== null && <DisplayHistory aanvragen={aanvragen} />}
            </div>
        </div>
    );
};