import { useState } from 'react';
import './emailButton.css';
import VerhuurAanvraag from '../interface/verhuuraanvraag';
type EmailListProps = {
    readonly emails: string[];
    readonly setAanvragen: React.Dispatch<React.SetStateAction<VerhuurAanvraag[] | null>>;
};

//laat alle gebruikers zien van een bedrijf met invoervelden voor maand en jaar zodat hiervan de verhuuraanvragen kunnen worden opgevraagd
export default function EmailList({ emails, setAanvragen }: EmailListProps) {
    const [selectedMonths, setSelectedMonths] = useState<{ [key: string]: string }>({});
    const [years, setYears] = useState<{ [key: string]: string }>({});
    const handleButtonClick = async (email: string) => {
        const maand = selectedMonths[email] || '';
        const jaar = years[email] || '';
        try {
            setAanvragen(null); // Set to null initially
            const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/Wagenparkbeheerder/GetVoertuigenPerUser`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify({ email, maand, jaar }),
            });
            if (response.status === 204) {
                setAanvragen([]);
                return;
            }
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const data = await response.json();
            setAanvragen(data);
            setSelectedMonths((prev) => ({ ...prev, [email]: '' }));
            setYears((prev) => ({ ...prev, [email]: '' }));
        } catch (error) {
            console.error('Error fetching emails:', error);
        }
    };
    const months = [
        'January', 'February', 'March', 'April', 'May', 'June',
        'July', 'August', 'September', 'October', 'November', 'December', 'Whole year'
    ];
    return (
        <div className="email-list">
            <h2>Huidige werknemers</h2>
            <ul>
                {emails.map((email) => {
                    const isButtonDisabled = !selectedMonths[email] || !years[email];
                    return (
                        <li key={email}>
                            {email.toLowerCase()}
                            <button
                                onClick={() => handleButtonClick(email)}
                                disabled={isButtonDisabled}
                                className={isButtonDisabled ? 'disabled' : ''}
                            >
                                Krijg huidige verhuuraanvragen
                            </button>
                            <label htmlFor={`month-select-${email}`}>Month:</label>
                            <select
                                id={`month-select-${email}`}
                                value={selectedMonths[email] || ''}
                                onChange={(e) => setSelectedMonths((prev) => ({ ...prev, [email]: e.target.value }))}
                            >
                                <option value="" disabled>Select a month</option>
                                {months.map((month) => (
                                    <option key={month} value={month}>{month}</option>
                                ))}
                            </select>
                            <label htmlFor={`year-input-${email}`}>Year:</label>
                            <input
                                id={`year-input-${email}`}
                                type="text"
                                value={years[email] || ''}
                                onChange={(e) => setYears((prev) => ({ ...prev, [email]: e.target.value }))}
                                placeholder="Enter year"
                            />
                        </li>
                    );
                })}
            </ul>
        </div>
    );
}