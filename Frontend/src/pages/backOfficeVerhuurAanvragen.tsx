import { useEffect, useState } from "react";
import "../styles/voertuigenOverview.css";

export default function VoertuigenOverview() {
  const [verhuurAanvragen, setVerhuurAanvragen] = useState([]);
  const [status, setStatus] = useState('');
  const [aanvraagID, setAanvraagID] = useState('');

  useEffect(() => {
    const fetchVerhuurAanvragen= async () => {
      try {
        const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/BackOffice/GetVerhuurAanvragen`, {
          credentials: "include"
      });
        const data = await response.json();
        setVerhuurAanvragen(data);
      } catch (error) {
        console.error("Error fetching verhuur aanvragen:", error);
      }
    };

    fetchVerhuurAanvragen();
  }, []);

  const handleVerhuurAanvraagStatusChange = async () => {
    try {
        const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/BackOffice/ChangeStatus`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'include',
            body: JSON.stringify({ aanvraagID: aanvraagID, status: status }),
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
            <h2>Verhuur aanvragen</h2>
        </div>
        <br/>
        <hr></hr>
        <br/>
        <section>
            <div>
        <table className="voertuigenTabel">
            <thead>
            <tr>
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
                <td>{verhuuraanvraag.startdatum}</td>
                <td>{verhuuraanvraag.einddatum}</td>
                <td>{verhuuraanvraag.bestemming}</td>
                <td>{verhuuraanvraag.kilometers}</td>
                <td>
                    <button onClick={ () => {
                        setStatus("Geaccepteerd");
                        setAanvraagID(verhuuraanvraag.aanvraagID);
                        handleVerhuurAanvraagStatusChange();
                    }
                         }>Accepteren</button>
                    <button onClick={ () => {
                        setStatus("Afgewezen");
                        setAanvraagID(verhuuraanvraag.aanvraagID);
                        handleVerhuurAanvraagStatusChange();
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