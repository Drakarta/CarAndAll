import { useEffect, useState } from "react";
import "../styles/abonnementen.css";

// Gedefineerd interface voor abonnementaanvragen
interface AbonnementAanvraag {
  AanvraagID: number;
  Naam: string;
  Beschrijving: string;
  PrijsMultiplier: number;
  MaxMedewerkers: number;
  Status: string;
}

export default function BackOfficeAbonnementAanvragen() {
  const [abonnementAanvragen, setAbonnementAanvragen] = useState<AbonnementAanvraag[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [status, setStatus] = useState<string>('');
  const [AanvraagIDset, setAanvraagID] = useState<number | null>(null);

  useEffect(() => {
    const fetchAbonnementAanvragen = async (): Promise<void> => {
      setLoading(true);
      setError(null);
// Fetch request om abonnement aanvragen op te halen
      try {
        const response = await fetch(
          `${import.meta.env.VITE_REACT_APP_API_URL}/AbonnementAanvraag/GetAbonnementAanvragen`,
          { credentials: "include" }
        );

        if(response.status === 404) {
          setError("Geen aanvragen");
          return;
        }

        if (!response.ok) {
          throw new Error("Failed to fetch abonnement aanvragen");
        }
// Data omzetten naar JSON formaat
        const data = await response.json();
        setAbonnementAanvragen(
          data.map((item: any) => ({
            AanvraagID: item.id,
            Naam: item.naam,
            Beschrijving: item.beschrijving,
            PrijsMultiplier: item.prijsMultiplier,
            MaxMedewerkers: item.maxMedewerkers,
            Status: item.status,
          }))
        );
      } catch (error) {
        if (error instanceof Error) {
          setError(error.message);
        } else {
          setError("An unknown error occurred");
        }
      } finally {
        setLoading(false);
      }
    };

    fetchAbonnementAanvragen();
  }, []);

  if (loading) {
    return <div className="loading">Laden...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }
// Functie om status van abonnement aanvraag te wijzigen
  const handleVerhuurAanvraagStatusChange = async () => {
    try {
      const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/AbonnementAanvraag/ChangeStatus`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify({ AanvraagID: AanvraagIDset, status: status }),
      });
      
      if (!response.ok) {
        const errorText = await response.text();
        console.log('Error response:' + response.status + errorText);
      } else {
        const data = await response.json();
        console.log('Success:', data);
        alert('Verhuur aanvraag status succesvol aangepast.');
      }
    } catch (error) {
      console.error('Error making verhuur aanvraag:', error);
    }
  };

  const handleClick = (aanvraagID: number, newStatus: string) => {
    setStatus(newStatus);
    setAanvraagID(aanvraagID);
    handleVerhuurAanvraagStatusChange();
  };
// Teruggeven van abonnement aanvragen
  return (
    <div className="abonnementenSection">
      <h2>Abonnement Aanvragen</h2>
      <table className="abonnementenTabel">
        <thead>
          <tr>
            <th>ID</th>
            <th>Naam</th>
            <th>Beschrijving</th>
            <th>Prijs Multiplier</th>
            <th>Max Medewerkers</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          {abonnementAanvragen.map((aanvraag) => (
            <tr key={aanvraag.AanvraagID}>
              <td>{aanvraag.AanvraagID}</td>
              <td>{aanvraag.Naam}</td>
              <td>{aanvraag.Beschrijving}</td>
              <td>{aanvraag.PrijsMultiplier.toFixed(2)}</td>
              <td>{aanvraag.MaxMedewerkers}</td>
              <td>{aanvraag.Status}</td>
              <td>
                <button onClick={() => handleClick(aanvraag.AanvraagID, "Geaccepteerd")}>Accepteren</button>
                <button onClick={() => handleClick(aanvraag.AanvraagID, "Afgewezen")}>Afwijzen</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}