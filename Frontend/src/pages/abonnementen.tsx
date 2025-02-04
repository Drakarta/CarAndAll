import React, { useEffect, useState } from "react";
import "../styles/abonnementen.css";
// Gedefineerd interface voor abonnementen
interface Abonnement {
  id: number;
  naam: string;
  prijs_multiplier: number;
  beschrijving: string;
  max_medewerkers: number;
}

const Abonnementen: React.FC = () => {
  const [abonnementen, setAbonnementen] = useState<Abonnement[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [currentAbonnement, setCurrentAbonnement] = useState<Abonnement | null>(null);
  const [bedrijfId, setBedrijfId] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);

      try {
        // Fetch bedrijfId
        const bedrijfResponse = await fetch(
          `${import.meta.env.VITE_REACT_APP_API_URL}/abonnement/currentBedrijf`,
          {
            method: "GET",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
          }
        );
        // Data omzetten naar JSON formaat
        if (bedrijfResponse.ok) {
          const bedrijfData = await bedrijfResponse.json();
          console.log("Fetched bedrijfId:", bedrijfData.bedrijfId); // Debugging statement
          setBedrijfId(bedrijfData.bedrijfId);
        } else {
          console.error("Error fetching bedrijfId:", await bedrijfResponse.text());
        }

        // Fetch abonnementen
        const abonnementenResponse = await fetch(
          `${import.meta.env.VITE_REACT_APP_API_URL}/abonnement`,
          {
            method: "GET",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
          }
        );

        if (abonnementenResponse.ok) {
          const abonnementenData = await abonnementenResponse.json();
          setAbonnementen(abonnementenData);
        } else {
          console.error("Error fetching abonnementen:", await abonnementenResponse.text());
        }

        // Fetch current abonnement
        const userAbonnementResponse = await fetch(
          `${import.meta.env.VITE_REACT_APP_API_URL}/Abonnement/id`,
          {
            method: "GET",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
          }
        );
        
        if (userAbonnementResponse.ok) {
          const userAbonnementData: Abonnement = await userAbonnementResponse.json();
          setCurrentAbonnement(userAbonnementData);
        } else {
          console.error("Error fetching current abonnement:", await userAbonnementResponse.text());
        }
      } catch (error) {
        console.error("Error fetching data:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);
  // Functie om abonnement te selecteren
  const handleSelectAbonnement = async (abonnement: Abonnement) => {
    console.log("Current bedrijfId:", bedrijfId); // Debugging statement
    if (!bedrijfId) {
      alert("Bedrijf ID is not available.");
      return;
    }
    // Functie om abonnement aan te vragen
    try {
      const response = await fetch(
        `${import.meta.env.VITE_REACT_APP_API_URL}/AbonnementAanvraag/create`, // Updated endpoint
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          credentials: "include",
          body: JSON.stringify({
            Naam: abonnement.naam,
          }),
        }
      );

      const result = await response.json();
      console.log("Response from server:", result); // Debugging statement

      if (response.ok) {
        alert(`Abonnement ${abonnement.naam} succesvol aangevraagd!`);
      } else {
        alert(`Fout: ${result.message}`);
      }
    } catch (error) {
      console.error("Error selecting abonnement:", error);
      alert("Er is een onverwachte fout opgetreden.");
    }
  };

  if (loading) {
    return <div className="loading">Laden...</div>;
  }
  // Teruggeven van abonnementen
  return (
    <div className="abonnementenSection">
      {currentAbonnement ? (
        <>
          <h1>Huidig Abonnement</h1>
          <p>
            U heeft al een abonnement: <strong>{currentAbonnement.naam}</strong>
          </p>
          <table className="abonnementenTabel">
            <thead>
              <tr>
                <th>Beschrijving</th>
                <th>Prijs (€/km)</th>
                <th>Max. Medewerkers</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td>{currentAbonnement.beschrijving}</td>
                <td>{currentAbonnement.prijs_multiplier.toFixed(2)}</td>
                <td>{currentAbonnement.max_medewerkers}</td>
              </tr>
            </tbody>
          </table>
        </>
      ) : (
        <h1>Abonnementen Overzicht</h1>
      )}
      <table className="abonnementenTabel">
        <thead>
          <tr>
            <th>Abonnement</th>
            <th>Beschrijving</th>
            <th>Prijs (€/km)</th>
            <th>Max. Medewerkers</th>
            <th>Acties</th>
          </tr>
        </thead>
        <tbody>
          {abonnementen.map((abonnement) => (
            <tr key={abonnement.id}>
              <td>{abonnement.naam}</td>
              <td>{abonnement.beschrijving}</td>
              <td>{abonnement.prijs_multiplier.toFixed(2)}</td>
              <td>{abonnement.max_medewerkers}</td>
              <td>
                <button
                  onClick={() => handleSelectAbonnement(abonnement)}
                  className="chooseButton"
                >
                  Kies
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default Abonnementen;