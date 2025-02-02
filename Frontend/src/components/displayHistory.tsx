import '../styles/EmailManager.css';
import VerhuurAanvraag from '../interface/verhuuraanvraag';

type DisplayHistoryProps = {
    readonly aanvragen: VerhuurAanvraag[];
};

//DisplayHistory laad de verhuuraanvragen van een specifieke gebruiker gebaseerdt op een bepaalde maand en jaar.
export default function DisplayHistory({ aanvragen }: DisplayHistoryProps) {
    if (aanvragen.length > 0) {
        const accountName = aanvragen[0].accountNaam || 'Unknown';
        return (
            <div className="history">
                <h3>{accountName} history</h3>
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
                        {aanvragen.map((verhuurAanvragen) => (
                            <tr key={`${verhuurAanvragen.AccountID}-${verhuurAanvragen.startdatum}`}>
                                <td>{verhuurAanvragen.startdatum}</td>
                                <td>{verhuurAanvragen.einddatum}</td>
                                <td>{verhuurAanvragen.bestemming}</td>
                                <td>{verhuurAanvragen.kilometers}</td>
                                <td>{verhuurAanvragen.status}</td>
                                <td>{verhuurAanvragen.voertuig.merk}</td>
                                <td>{verhuurAanvragen.voertuig.type}</td>
                                <td>{verhuurAanvragen.voertuig.prijs_per_dag}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        );
    } else {
        return (
            <div className="history">
                <p>User has no history</p>
            </div>
        );
    }
}