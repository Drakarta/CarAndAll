import { useEffect, useState } from "react";
import "../styles/voertuigenOverview.css";
import VerhuurAanvraag from "./verhuurAanvraag";

interface VerhuurAanvraag{
  AanvraagID: number;
  startdatum: string;
  einddatum: string;
  bestemming: string;
  kilometers: number;
}

export default function VoertuigenOverview() {
  //Verhuuraanvragen lijst
  const [verhuurAanvragen, setVerhuurAanvragen] = useState<VerhuurAanvraag[]>([]);

  //useEffect voor het ophalen van de opgehaalde verhuuraanvragen uit backend/db
  useEffect(() => {
    const fetchVerhuurAanvragen= async () => {
      try {
        const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/BackOffice/GetVerhuurAanvragen`, {
          credentials: "include"
      });
      if (response.status === 405) {
        window.location.href = "/404";
    }
        const data = await response.json();
        setVerhuurAanvragen(data.map((item: any) => ({
          AanvraagID: item.aanvraagID,
          Status: item.status,
          startdatum: item.startdatum,
          einddatum: item.einddatum,
          bestemming: item.bestemming,
          kilometers: item.kilometers
        })));
      } catch (error) {
        console.error("Error fetching verhuur aanvragen:", error);
      }
    };

    fetchVerhuurAanvragen();
  }, []);

  //Verhuuraanvragen status change voor het aanpassen van de status naar geaccepteerd/ afgewezen
  const handleVerhuurAanvraagStatusChange = async (verhuurAanvraagID: number, verhuurAanvraagStatus: string) => {
    try {
        const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/BackOffice/ChangeStatus`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'include',
            body: JSON.stringify({ AanvraagID: verhuurAanvraagID, status: verhuurAanvraagStatus }),
        });

        if (!response.ok) {
            const errorText = await response.text();
            console.log('Error response:' + response.status + errorText);
        } else {
            const data = await response.json();
            console.log('Success:', data);
            alert('Verhuur aanvraag status succesvol aangepast.');
        }

    }catch (error) {
        console.error('Error making verhuur aanvraag:', error);
    }
  }

  return (
    <>
       <div className="overviewSection">
        <div className="headerFilter">
            <h1>Verhuur aanvragen</h1>
        </div>
        <br/>
        <hr></hr>
        <br/>
        <section>
            <div>
        <table className="voertuigenTabel">
            <thead>
            <tr>
              <th>AanvraagID</th>
              <th>Start datum</th>
              <th>Eind datum</th>
              <th>Bestemming</th>
              <th>Kilometers</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
          {verhuurAanvragen.map((verhuuraanvraag, index) => (
              <tr key={index}>
                <td>{verhuuraanvraag.AanvraagID}</td>
                <td>{verhuuraanvraag.startdatum}</td>
                <td>{verhuuraanvraag.einddatum}</td>
                <td>{verhuuraanvraag.bestemming}</td>
                <td>{verhuuraanvraag.kilometers}</td>
                <td>
                    <button onClick={ () => {
                        handleVerhuurAanvraagStatusChange(verhuuraanvraag.AanvraagID, "Geaccepteerd");
                    }
                         }>Accepteren</button>
                    <button onClick={ () => {
                        handleVerhuurAanvraagStatusChange(verhuuraanvraag.AanvraagID, "Afgewezen");
                    }
                         }>Afwijzen</button>
                </td>
            </tr>
            ))}
          </tbody>
          </table>
            </div>
        </section>
    </div>
   </>
  );
}