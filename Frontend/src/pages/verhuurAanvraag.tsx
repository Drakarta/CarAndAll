import { useParams, useNavigate } from "react-router-dom";
import "../styles/voertuigenOverview.css";
import { useState, useEffect } from "react";

//interface voor accessoire voor betere duidelijkheid over wat een accessoire heeft
interface Accessoire {
    accessoireNaam: string;
    extra_prijs_per_dag: number;
    max_aantal: number;
}

//interface voor een denkbeeldige accessoire lijst voor duidelijkheid over wat in de lijst mag
interface AccessoireList {
    accessoireNaam: string;
    aantal: number;
}

export default function VerhuurAanvraag() {

    //Accessoires lijst
    const [Accessoires, setAccessoires] =  useState<Accessoire[]>([]);

    //Voertuig variablen en start/eind datum
    const {voertuigID, voertuigNaam, voertuigPrijs, vastartdate, vaenddate} = useParams();

    //Verhuuraanvraag variablen
    const [bestemming, setBestemming] = useState('');
    const [kilometers, setKilometers] = useState(0);
    const [selectedVerzekering, setSelectedVerzekering] = useState<number>(1.0);
    const [selectedAccessoires, setSelectedAccessoires] = useState<AccessoireList[]>([]);

    //Navigatie
    const navigate = useNavigate();

    //useEffect voor het ophalen van de accessoires uit de backend/db
      useEffect(() => {
        const fetchAccessoires = async () => {
          try {
            const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/VerhuurAanvraag/GetAccessoires`, {
              credentials: "include"
          });
          if (response.status === 405) {
            window.location.href = "/404";
        }
            const data = await response.json();
            setAccessoires(data);
          } catch (error) {
            console.error("Error fetching accessoires:", error);
          }
        };
    
        fetchAccessoires();
      }, []);

    //Voor het aanmaken van de verhuuraanvraag/verhuuraanvragen een post naar backend met alle benodigde data
    const handleMakeVerhuurAanvraag = async () => {
        try {
            const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/VerhuurAanvraag/createVerhuurAanvraag`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify({ startdatum: vastartdate, einddatum: vaenddate, bestemming: bestemming, kilometers: kilometers, verzekering_multiplier: selectedVerzekering, voertuigID: voertuigID, accessoires: selectedAccessoires}),
            });
            if (response.status === 405) {
                window.location.href = "/404";
            }

            let responseData;
            const contentType = response.headers.get('Content-Type');
            if (contentType && contentType.includes('application/json')) {
                responseData = await response.json();
            } else {
                responseData = { message: await response.text() };
            }

            if (!response.ok) {
                if(responseData.statusCode === 400){
                    alert(responseData.message);
                }
            } else {
                alert('Verhuur aanvraag succesvol ingediend.');
                navigate('/voertuigenOverview');
            }

        }catch (error) {
            console.error('Error making verhuur aanvraag:', error);
        }

    }

    //handler voor het toevoegen van accessoires en aanpassen van accessoires aan het verhuuraanvraag
    const handleAccessoireChange = (accessoireNameChange: string, accessoireNewAantal: number) => {
        const tempAccessoire = selectedAccessoires.find(a => a.accessoireNaam === accessoireNameChange);
        if(tempAccessoire){
            tempAccessoire.aantal = accessoireNewAantal;
        }else{
            const insertAt = 1;
            const nieuweAccessoire = [
              ...selectedAccessoires.slice(0, insertAt),
              { accessoireNaam: accessoireNameChange, aantal: accessoireNewAantal },
              ...selectedAccessoires.slice(insertAt)
            ];
            setSelectedAccessoires(nieuweAccessoire);
        }
    }
return (
    <>
    <div className="overviewSection">
        <div className="headerFilter">
            <h1>Verhuur aanvraag</h1>
        </div>
        <br/>
        <hr></hr>
        <br/>
        <section>
            <div className="allInputs">
            <div>
                <label>Bestemming: 
                <input id="bestemming" value={bestemming} onChange={(e) => setBestemming(e.target.value)} required/></label>
                <br/>
                <label>Kilometers: 
                <input type="number" id="kilometers" value={kilometers} onChange={(e) => setKilometers(parseInt(e.target.value))} required/></label>
                <br/>
                <label>Verzekering: <select id="selectedPriceFilter" value={selectedVerzekering} 
                    onChange={(e) => setSelectedVerzekering(parseFloat(e.target.value))}>
                    <option value={1.0}>Geen verzekering (1x)</option>
                    <option value={1.75}>Eigen risico €200 (1.75x)</option>
                    <option value={1.50}>Eigen risico €400 (1.50x)</option>
                    <option value={1.25}>Eigen risico €600 (1.25x)</option>
                    <option value={2.0}>Eigen risico €0 (2x)</option>
                </select></label>
                <p><b>Voertuig: </b>{voertuigNaam}</p>
                <p><b>Start datum: </b>{vastartdate}</p>
                <p><b>Eind datum: </b>{vaenddate}</p>
                <p><b>Voertuig prijs per dag: </b>{ (parseFloat(voertuigPrijs?.toString() || "0") * selectedVerzekering)} + enige accessoire kosten</p>
                <button onClick={handleMakeVerhuurAanvraag}>Verhuur aanvraag indienen</button>
            </div>
            <div>
                <p><b>Accessoire lijst:</b></p>
                {Accessoires.length > 0 ? (
                Accessoires.map((accessoire, index) => (

                    <label key={index} htmlFor={accessoire.accessoireNaam + index}>{accessoire.accessoireNaam}(Max: {accessoire.max_aantal})(+€{accessoire.extra_prijs_per_dag}):
                        <input id={accessoire.accessoireNaam + index} type="number" min={0} max={accessoire.max_aantal} defaultValue={0} 
                            onChange={ (e) => handleAccessoireChange(accessoire.accessoireNaam, parseInt(e.target.value))}/>
                        <br/>
                    </label>
                    
                    ))

                ) : (
                  <div>
                    <b>No results found</b>
                  </div>
                )}
                <p>1 voor banden = alle banden</p>
            </div>
            </div>
        </section>
    </div>
    </>
  );
}
