import { useEffect, useState } from 'react';
import '../styles/klantAanvragen.css';
import KlantAanvraag from '../interface/KlantAanvraag';

//Hier worden alle klant aanvragen opgehaald en weergegeven. Ook worden de kosten berekend en weergegeven(voor voertuig * dag, Accessoires * dag en de verzekering).

export default function KlantAanvragen() {

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
            if (response.status === 405) {
                window.location.href = "/404";
            } 

            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const data = await response.json();
            setKlantAanvragen(data);
        } catch (error) {
            console.error('Error fetching Klant Aanvragen:', error);
        }
    };

    const calculateDays = (startdatum: string, einddatum: string) => {
        return (new Date(einddatum).getTime() - new Date(startdatum).getTime()) / (1000 * 3600 * 24);
    };

    const calculateVehiclePrice = (aanvraag: KlantAanvraag, days: number) => {
        return aanvraag.voertuig.prijs_per_dag * days;
    };

    const calculateAccessoriesPrice = (aanvraag: KlantAanvraag, days: number) => {
        return aanvraag.verhuurAanvraagAccessoires?.reduce((total, accessory) => total + (accessory.extraPrijsPerDag * accessory.aantal * days), 0) || 0;
    };

    const calculateInsurancePrice = (vehiclePrice: number, verzekering_multiplier: number) => {
        return vehiclePrice * (verzekering_multiplier - 1);
    };

    const calculateTotalPrice = (vehiclePrice: number, accessoriesPrice: number, insurancePrice: number) => {
        return vehiclePrice + accessoriesPrice + insurancePrice;
    };

    return (
        <div className="klant-aanvragen-box">
            <div className="klant-aanvragen">
                <div className="header">
                    <h1>
                    Klant Aanvragen
                    </h1>
                </div>
                <div className="content">
                    {klantAanvragen.length === 0 ? (
                        <p>No klant aanvragen found.</p>
                    ) : (
                        <ul>
                            {klantAanvragen?.map((aanvraag) => {
                                const days = calculateDays(aanvraag.startdatum, aanvraag.einddatum);
                                const vehiclePrice = calculateVehiclePrice(aanvraag, days);
                                const accessoriesPrice = calculateAccessoriesPrice(aanvraag, days);
                                const insurancePrice = calculateInsurancePrice(vehiclePrice, aanvraag.verzekering_multiplier);
                                const totalPrice = calculateTotalPrice(vehiclePrice, accessoriesPrice, insurancePrice);

                                return (
                                    <li key={`${aanvraag.id}-${aanvraag.startdatum}`} className="aanvraag-row">
                                        {aanvraag.startdatum} - {aanvraag.einddatum} - {aanvraag.bestemming} - {aanvraag.kilometers} - {aanvraag.voertuig.type} - {aanvraag.voertuig.merk} - {aanvraag.status}
                                        <ul>
                                            <li>Voertuig Kosten: €{vehiclePrice.toFixed(2)}</li>
                                            <li>Accessoires Kosten: €{accessoriesPrice.toFixed(2)}</li>
                                            <li>Verzekering: {aanvraag.verzekering_multiplier > 1 ? 'Yes' : 'No'} - €{insurancePrice.toFixed(2)}</li>
                                            <li>Totale Prijs: €{totalPrice.toFixed(2)}</li>
                                        </ul>
                                    </li>
                                );
                            })}
                        </ul>
                    )}
                </div>
            </div>
        </div>
    );
};