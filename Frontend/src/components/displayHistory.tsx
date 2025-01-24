import '../styles/EmailManager.css';
import VerhuurAanvraag from '../interface/verhuuraanvraag';

type DisplayHistoryProps = {
    readonly aanvragen: VerhuurAanvraag[];
};

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
                            <tr key={verhuurAanvragen.AccountID}>
                                <th>{verhuurAanvragen.startdatum}</th>
                                <th>{verhuurAanvragen.einddatum}</th>
                                <th>{verhuurAanvragen.bestemming}</th>
                                <th>{verhuurAanvragen.kilometers}</th>
                                <th>{verhuurAanvragen.status}</th>
                                <th>{verhuurAanvragen.voertuig.merk}</th>
                                <th>{verhuurAanvragen.voertuig.type}</th>
                                <th>{verhuurAanvragen.voertuig.prijs_per_dag}</th>
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