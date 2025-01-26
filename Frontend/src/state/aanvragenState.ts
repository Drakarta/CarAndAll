import { useState } from 'react';
import VerhuurAanvraag from '../interface/verhuuraanvraag';

export function useAanvragenState() {
    const [aanvragen, setAanvragen] = useState<VerhuurAanvraag[] | null>(null);
    return { aanvragen, setAanvragen };
}
