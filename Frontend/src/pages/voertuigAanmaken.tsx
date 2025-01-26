import { useNavigate } from "react-router-dom";
import "../styles/voertuigenOverview.css";
import { useState } from "react";

export default function voertuigAanmaken() {
    //voertuig variablen
    const [merk, setMerk] = useState('');
    const [type, setType] = useState('');
    const [kenteken, setKenteken] = useState('');
    const [kleur, setKleur] = useState('');
    const [aanschafjaar, setAanschafjaar] = useState('');
    const [prijs, setPrijs] = useState(0);
    const [status, setStatus] = useState('Beschikbaar');

    //auto variablen
    const [aantalDeuren, setAantalDeuren] = useState(0);
    const [elektrisch, setElektrisch] = useState(false);

    //camper en camper variablen
    const [aantalSlaapplekken, setAantalSlaapplekken] = useState(0);

    //caravan variablen
    const [gewicht, setGewicht] = useState(0);

    //voertuig soort
    const [selectedVoertuig, setSelectedVoertuig] = useState("Null");

    //navigatie
    const navigate = useNavigate();

    //post naar backend voor maken van voertuig
    const handleMakeVoertuig = async () => {
        try {
            const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/Voertuig/createVoertuig`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify({ merk: merk, type: type, kenteken: kenteken, kleur: kleur, aanschafjaar: aanschafjaar, prijs_per_dag: prijs, aantal_deuren: aantalDeuren, elektrisch: elektrisch, aantal_slaapplekken: aantalSlaapplekken, gewicht_kg: gewicht, voertuig_categorie: selectedVoertuig, status: status}),
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
                alert('Voertuig succesvol aangemaakt');
                navigate('/voertuigenOverview');
            }

        }catch (error) {
            console.error('Error making voertuig:', error);
        }
    }

    //form changer aan de hand van voertuig soort (auto, camper, caravan)
    const changeForm = (selectedValue: string) => {
        setSelectedVoertuig(selectedValue);

        const elementIDs = ["Null", "Auto", "Caravan", "Camper"];

        elementIDs.forEach(id => {
            const element = document.getElementById(id);
            if (element) {
              element.style.display = 'none';
            }
          });

        const selectedElement = document.getElementById(selectedValue);
        if (selectedElement) {
            selectedElement.style.display = 'block';
        }
      }
return (
    <>
    <div className="overviewSection">
        <div className="headerFilter">
            <h2>Voertuig aanmaken</h2>
        </div>
        <br/>
        <hr></hr>
        <br/>
        <section>
            <div className="createForm">
                <label>* Voertuig soort: 
                <select id="categorieFilter" value={selectedVoertuig} 
                onChange={(e) => changeForm(e.target.value)}>
                <option value="Null">Voertuigen</option>
                <option value="Auto">Auto</option>
                <option value="Caravan">Caravan</option>
                <option value="Camper">Camper</option>
                </select></label>

                <label>* Voertuig status:
                <select id="status" value={status} 
                onChange={(e) => setStatus(e.target.value)}>
                <option value="Beschikbaar">Beschikbaar</option>
                <option value="in reparatie">In reparatie</option>
                </select></label>
                <br/>
                <div className="allInputs">
                <div className="basicInputs">
                <label>* Merk: 
                <input id="merk" value={merk} onChange={(e) => setMerk(e.target.value)} required/></label>
                <br/>
                <label>* Type: 
                <input id="type" value={type} onChange={(e) => setType(e.target.value)} required/></label>
                <br/>
                <label>* Kenteken: 
                <input id="kenteken" value={kenteken} onChange={(e) => setKenteken(e.target.value)} required/></label>
                <br/>
                <label>* Kleur: 
                <input id="kleur" value={kleur} onChange={(e) => setKleur(e.target.value)} required/></label>
                <br/>
                <label>* Aanschafjaar: 
                <input id="aanschafjaar" value={aanschafjaar} onChange={(e) => setAanschafjaar(e.target.value)} required/></label>
                <br/>
                <label>* Prijs per dag: 
                <input type="number" id="prijs" value={prijs} onChange={(e) => setPrijs(parseInt(e.target.value))} required/></label>
                <br/>
                </div>
                <div className="advancedInputs">
                    <div id="Auto">
                        <p><b>Auto</b></p>
                        <br/>
                        <label>* Aantal deuren: 
                        <input type="number" id="aantal_deuren" value={aantalDeuren} onChange={(e) => setAantalDeuren(parseInt(e.target.value))} required/></label>
                        <label>* Elektrisch: 
                        <input type="checkbox" id="elektrisch" checked={elektrisch} onChange={(e) => setElektrisch(e.target.checked)} required/></label>
                    </div>
                    <div id="Camper">
                        <p><b>Camper</b></p>
                        <br/>
                        <label>* Aantal slaapplekken: 
                        <input type="number" id="aantal_slaapplekken" value={aantalSlaapplekken} onChange={(e) => setAantalSlaapplekken(parseInt(e.target.value))} required/></label>
                        <label>* Elektrisch: 
                        <input type="checkbox" id="elektrisch" checked={elektrisch} onChange={(e) => setElektrisch(e.target.checked)} required/></label>
                        </div>
                    <div id="Caravan">
                        <p><b>Caravan</b></p>
                        <label>* Aantal slaapplekken: 
                        <input type="number" id="aantal_slaapplekken" value={aantalSlaapplekken} onChange={(e) => setAantalSlaapplekken(parseInt(e.target.value))} required/></label>
                        <label>* Gewicht in KG: 
                        <input type="number" id="gewicht" value={gewicht} onChange={(e) => setGewicht(parseInt(e.target.value))} required/></label>
                        </div>
                    <div id="Null"><p>Kies een soort voertuig</p></div>
                    
                </div>
                </div>
                <button onClick={handleMakeVoertuig}>Voertuig aanmaken</button>
            </div>
        </section>
    </div>
    </>
  );
}
