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

  useEffect(() => {
    const fetchAbonnementAanvragen = async () => {
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
        console.error("Error fetching abonnement aanvragen:", error);
      }
    };

    fetchAbonnementAanvragen();
  }, []);

  const handleAbonnementAanvraagStatusChange = async (id: number, newStatus: string) => {
    if (!newStatus || !id) {
      alert("Aanvraag ID of status ontbreekt!");
      return;
    }

    try {
      const response = await fetch(
        `${import.meta.env.VITE_REACT_APP_API_URL}/AbonnementAanvraag/ChangeStatus`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          credentials: "include",
          body: JSON.stringify({ AanvraagID: id, status: newStatus }),
        }
      );

      if (!response.ok) {
        const errorText = await response.text();
        alert(`Fout bij het wijzigen van de status: ${errorText}`);
        return;
      }

      setAbonnementAanvragen((prev) =>
        prev.map((aanvraag) =>
          aanvraag.AanvraagID === id ? { ...aanvraag, Status: newStatus } : aanvraag
        )
      );

      alert("Abonnement aanvraag status succesvol aangepast.");
    } catch (error) {
      console.error("Error updating abonnement aanvraag status:", error);
      alert("Er is een fout opgetreden bij het aanpassen van de status.");
    }
  };

  return (
    <div className="overviewSection">
      <div className="headerFilter">
        <h2>Abonnement Aanvragen</h2>
      </div>
      <br />
      <hr />
      <br />
      <section>
        <div>
          <table className="abonnementTabel">
            <thead>
              <tr>
                <th>AanvraagID</th>
                <th>Naam</th>
                <th>Beschrijving</th>
                <th>Prijs Multiplier</th>
                <th>Max Medewerkers</th>
                <th>Status</th>
                <th>Acties</th>
              </tr>
            </thead>
            <tbody>
              {abonnementAanvragen.map((aanvraag) => (
                <tr key={aanvraag.AanvraagID}>
                  <td>{aanvraag.AanvraagID}</td>
                  <td>{aanvraag.Naam}</td>
                  <td>{aanvraag.Beschrijving}</td>
                  <td>{aanvraag.PrijsMultiplier}</td>
                  <td>{aanvraag.MaxMedewerkers}</td>
                  <td>{aanvraag.Status}</td>
                  <td>
                    <button
                      onClick={() =>
                        handleAbonnementAanvraagStatusChange(aanvraag.AanvraagID, "Geaccepteerd")
                      }
                    >
                      Accepteren
                    </button>
                    <button
                      onClick={() =>
                        handleAbonnementAanvraagStatusChange(aanvraag.AanvraagID, "Afgewezen")
                      }
                    >
                      Afwijzen
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  );
}
