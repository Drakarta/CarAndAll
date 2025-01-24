import { useEffect, useState } from 'react';
import '../styles/klantAanvragen.css';

export default function KlantAanvragen() {
    interface Voertuig {
        type: string;
        merk: string;
    }

    interface KlantAanvraag {
        id: number;
        startdatum: string;
        einddatum: string;
        bestemming: string;
        kilometers: number;
        voertuig: Voertuig;
        status: string;
    }

    const [klantAanvragen, setKlantAanvragen] = useState<KlantAanvraag[]>([]);

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
        <div className="klant-aanvragen-box">
            <div className="klant-aanvragen">
                <div className="header">
                    Klant Aanvragen
                </div>
                <div className="content">
                    {klantAanvragen.length === 0 ? (
                        <p>No klant aanvragen found.</p>
                    ) : (
                        <ul>
                        {klantAanvragen?.map((aanvraag) => (
                            <li key={aanvraag.id} className="aanvraag-row">
                                {aanvraag.startdatum} - {aanvraag.einddatum} - {aanvraag.bestemming} - {aanvraag.kilometers} - {aanvraag.voertuig.type} - {aanvraag.voertuig.merk} - {aanvraag.status}
                        </li>
                        ))}
                    </ul>
                    )}
                </div>
            </div>
        </div>
    );
};

