import React, { useState, useEffect } from 'react';


const KlantAanvragen: React.FC = () => {
    const [klantAanvragen, setKlantAanvragen] = useState<any[]>([]);


    useEffect(() => {
        fetchAanvragen();
    }, []);

    const fetchAanvragen = async () => {
        try {
            const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/KlantAanvraag/GetKlantAanvragen`, {
                method: 'GET',
                credentials: 'include',
            });
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const data = await response.json();
            setKlantAanvragen(data);
        } catch (error) {
            console.error('Error fetching Klant Aanvragen:', error);
        }
    };

    return (
        <div className="klant-aanvragen">
            <div className="header">
                Klant Aanvragen
            </div>
            <div>
                <ul>
                    {klantAanvragen.map((aanvraag, index) => (
                        <li key={index}>
                            {aanvraag.begindatum} - {aanvraag.einddatum} - {aanvraag.bestemming} - {aanvraag.kilometers} - {aanvraag.voertuig.type} - {aanvraag.voertuig.merk} - {aanvraag.status}
                        </li>
                    ))}
                </ul>
            </div>
        </div>
    );
};

export default KlantAanvragen;
