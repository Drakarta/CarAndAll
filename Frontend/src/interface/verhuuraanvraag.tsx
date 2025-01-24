import VerhuurAanvraag from "../pages/verhuurAanvraag";

interface VerhuurAanvraag {
    startdatum: string;
    einddatum: string;
    bestemming: string;
    kilometers: number;
    status: string;
    voertuig: {
        merk: string;
        type: string;
        prijs_per_dag: number;
    };
    AccountID: string;
    accountNaam: string;
}

export default VerhuurAanvraag;