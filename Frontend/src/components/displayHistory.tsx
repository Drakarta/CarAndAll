import React from 'react';
import '../styles/EmailManager.css';

type DisplayHistoryProps = {
    aanvragen: string[];
};

export default function DisplayHistory({ aanvragen }: DisplayHistoryProps) {
    if (aanvragen.length > 0) {
    return (
        <div>
            <h3>user history</h3>
            <table className="aanvragenTabel">
            <thead>
                <tr>
                    <th>Start date</th>
                    <th>End date</th>
                    <th>Destination</th>
                    <th>Kilometers</th>
                    <th>Status</th>
                    <th>Brand</th>
                    <th>Type</th>
                    <th>Price per day</th>
                </tr>
            </thead>
            <tbody>
                {aanvragen.map((verhuurAanvragen, index) => (
                    <tr key={index}>
                        <th>{verhuurAanvragen.startdatum}</th> <th>{verhuurAanvragen.einddatum}</th> <th>{verhuurAanvragen.bestemming}</th><th> {verhuurAanvragen.kilometers}</th> <th>{verhuurAanvragen.status}</th> <th>{verhuurAanvragen.voertuig.merk}</th> <th>{verhuurAanvragen.voertuig.type}</th><th> {verhuurAanvragen.voertuig.prijs_per_dag}</th>
                    </tr>
                ))}
            </tbody>
            </table>
        </div>
    );
    return null;
}
};