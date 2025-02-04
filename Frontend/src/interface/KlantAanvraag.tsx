import Voertuig from './Voertuig';

interface VerhuurAanvraagAccessoire {
    accessoireNaam: string;
    aantal: number;
    extraPrijsPerDag: number;
    maxAantal: number;
}

interface KlantAanvraag {
    id: number;
    startdatum: string;
    einddatum: string;
    bestemming: string;
    kilometers: number;
    voertuig: Voertuig;
    status: string;
    verzekering_multiplier: number;
    verhuurAanvraagAccessoires: VerhuurAanvraagAccessoire[];
}

export default KlantAanvraag;