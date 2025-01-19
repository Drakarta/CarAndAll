import { useState } from 'react';


type EmailListProps = {
    emails: string[];
    setAanvragen: (aanvragen: string[]) => void;
};

export default function EmailList({emails, setAanvragen}: EmailListProps) {
    const [maand, setSelectedMonth] = useState<string>('');
    const [jaar, setYear] = useState<string>('');

    const handleButtonClick = async (email: string) => {
        try {
            const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/Wagenparkbeheerder/GetVoertuigenPerUser`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify({ email, maand, jaar }),
            });
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const data = await response.json();
            setAanvragen(data);
            setSelectedMonth('');
            setYear('');
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
            <h2>Current Emails</h2>
            <ul>
                {emails.map((email, index) => (
                    <li key={index}>
                        {email.toLowerCase()}
                        <button onClick={() => handleButtonClick(email)}>Fetch</button>
                        <select value={maand} onChange={(e) => setSelectedMonth(e.target.value)}>
                        <option value="" disabled>Select a month</option>
                        {months.map((month, index) => (
                            <option key={index} value={month}>{month}</option>
                        ))}

                    </select>
                    <input
                        type="text"
                        value={jaar}
                        onChange={(e) => setYear(e.target.value)}
                        placeholder="Enter year"
                    />
                    </li>
                ))}
            </ul>
        </div>
    );
}