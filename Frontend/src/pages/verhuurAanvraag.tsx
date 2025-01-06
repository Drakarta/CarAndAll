import { useParams, useNavigate } from "react-router-dom";
import "../styles/voertuigenOverview.css";
import { useState } from "react";

export default function VerhuurAanvraag() {
    const {voertuigID, voertuigNaam, vastartdate, vaenddate} = useParams();
    const [bestemming, setBestemming] = useState('');
    const [kilometers, setKilometers] = useState(0);
    const navigate = useNavigate();

    const handleMakeVerhuurAanvraag = async () => {
        try {
            const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/VerhuurAanvraag/createVerhuurAanvraag`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify({ startdatum: vastartdate, einddatum: vaenddate, bestemming: bestemming, kilometers: kilometers, voertuigID: voertuigID}),
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.log('Error response:' + response.status + errorText);
            } else {
                const data = await response.json();
                console.log('Success:', data);
                alert('Verhuur aanvraag succesvol ingediend.');
                navigate('/voertuigenOverview');
            }

        }catch (error) {
            console.error('Error making verhuur aanvraag:', error);
        }

    }
return (
    <>
    <div className="overviewSection">
        <div className="headerFilter">
            <h2>Verhuur aanvraag</h2>
        </div>
        <br/>
        <hr></hr>
        <br/>
        <section>
            <div>
                <label>Bestemming: 
                <input id="bestemming" value={bestemming} onChange={(e) => setBestemming(e.target.value)} required/></label>
                <br/>
                <label>Kilometers: 
                <input type="number" id="kilometers" value={kilometers} onChange={(e) => setKilometers(parseInt(e.target.value))} required/></label>
                <p><b>Voertuig: </b>{voertuigNaam}</p>
                <p><b>Start datum: </b>{vastartdate}</p>
                <p><b>Eind datum: </b>{vaenddate}</p>
                <button onClick={handleMakeVerhuurAanvraag}>Verhuur aanvraag indienen</button>
            </div>
        </section>
    </div>
    </>
  );
}
