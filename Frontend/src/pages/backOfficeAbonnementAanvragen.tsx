import { useEffect, useState } from "react";
import "../styles/abonnementen.css";

// Define the interface for abonnement aanvragen
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

  useEffect(() => {
    const fetchAbonnementAanvragen = async () => {
      setLoading(true);
      setError(null);

      try {
        const response = await fetch(
          `${import.meta.env.VITE_REACT_APP_API_URL}/AbonnementAanvraag/GetAbonnementAanvragen`,
          { credentials: "include" }
        );

        if (!response.ok) {
          throw new Error("Failed to fetch abonnement aanvragen");
        }

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
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}