import Voertuig from './Voertuig';

interface KlantAanvraag {
    id: number;
    startdatum: string;
    einddatum: string;
    bestemming: string;
    kilometers: number;
    voertuig: Voertuig;
    status: string;
}

export default KlantAanvraag;