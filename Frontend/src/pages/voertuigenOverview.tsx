
import { useEffect, useState } from "react";
import {useNavigate } from 'react-router-dom';

//React datepicker voor betere handling en daterange
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import "../styles/voertuigenOverview.css";

import { useTokenStore } from "../stores";

interface Voertuig {
  voertuigID: number;
  naam: string;
  status: string;
  prijs_per_dag: number;
  voertuig_categorie: string;
  verhuur_perioden: {
    startDatum: Date;
    eindDatum: Date;
  }
}

export default function VoertuigenOverview() {
  
  //Voertuigen lijst
  const [Voertuigen, setVoertuigen] =  useState<Voertuig[]>([]);

  //Filter variablen
  const [selectedCategory, setSelectedCategory] = useState("Alle");
  const [isDisabled, setIsDisabled] = useState(false);
  const [selectedPriceFilter, setSelectedPriceFilter] = useState("none");
  const [selectedNameFilter, setSelectedNameFilter] = useState("");
  const [dateRange, setDateRange] = useState<[Date | null, Date | null]>([null, null]);
  const [startDate, endDate] = dateRange;
  const [verhuurAanvraagStartDate, setVerhuurAanvraagStartDate] = useState<Date | null>(null);
  const [verhuurAanvraagEndDate, setVerhuurAanvraagEndDate] = useState<Date | null>(null);
  const verhuurAanvraagDateRange = [verhuurAanvraagStartDate !== null ? new Date(verhuurAanvraagStartDate).toLocaleDateString("nl-NL"): null, 
                                    verhuurAanvraagEndDate !== null ? new Date(verhuurAanvraagEndDate).toLocaleDateString("nl-NL"): null]
  const navigate = useNavigate();
  const role = useTokenStore((state) => state.role) || "";

  //useEffect voor het ophalen van de voertuigen uit de backend/db
  useEffect(() => {
    const fetchVoertuigen = async () => {
      try {
        const response = await fetch(`${import.meta.env.VITE_REACT_APP_VOERTUIGEN_API_URL}`, {
          credentials: "include"
      });
        const data = await response.json();
        setVoertuigen(data);
      } catch (error) {
        console.error("Error fetching voertuigen:", error);
      }
    };

    fetchVoertuigen();
  }, []);

// Op basis van (.filter() kopje): https://medium.com/poka-techblog/simplify-your-javascript-use-map-reduce-and-filter-bd02c593cc2d
  const filterResultaat = Voertuigen.filter((voertuig) => {

    const categoryFilter = selectedCategory === "Alle" || voertuig.voertuig_categorie === selectedCategory;

    let statusFilter = true;
    if(role == "Particuliere huurder" || role == "Zakelijkeklant"){
      statusFilter = voertuig.status === "Beschikbaar";
      console.log(statusFilter)
    }

    if (!startDate || !endDate) {
      return categoryFilter && statusFilter;
    }

    const filterStartdatum = new Date(startDate);
    const filterEinddatum = new Date(endDate);

    const verhuurAanvraagOverlap = voertuig.verhuur_perioden.some((periode: { verhuur_start: string | number | Date; verhuur_eind: string | number | Date; }) => {
      const verhuurStart = new Date(periode.verhuur_start);
      const verhuurEnd = new Date(periode.verhuur_eind);

      return !(filterEinddatum < verhuurStart || filterStartdatum > verhuurEnd);
    });

  return categoryFilter && statusFilter && !verhuurAanvraagOverlap;
}).filter((voertuig) => voertuig.naam.toLowerCase()
.includes(selectedNameFilter.toLowerCase())).sort((a, b) => a.naam.localeCompare(b.naam));

// Datums die al bezit zijn ophalen
  const getExcludedDates = (verhuurPerioden: { startDatum: Date; eindDatum: Date; }) => {
    const excludedDates: Date[] = [];
    verhuurPerioden.forEach(periode => {
      const startdatum = new Date(periode.verhuur_start);
      const eindatum = new Date(periode.verhuur_eind);

      let loopDatum = new Date(startdatum);
      while (loopDatum <= eindatum) {
        excludedDates.push(new Date(loopDatum));
        loopDatum.setDate(loopDatum.getDate() + 1);
      }
    });
    return excludedDates;
  };

  //Checken of datums vrij of bezet zijn in vergelijking met de gebruikers keuzes
  const checkDates = (startDatum, eindDatum, verhuurPerioden: { startDatum: Date; eindDatum: Date; }) => {
    const pickedDates: Date[] = [];
    const excludedDates = getExcludedDates(verhuurPerioden)

      let loopDatum = new Date(startDatum);
      while (loopDatum <= eindDatum) {
        pickedDates.push(new Date(loopDatum));
        loopDatum.setDate(loopDatum.getDate() + 1);
      }
    return excludedDates.some(excludedDate => pickedDates.some(pickedDate => excludedDate.getDate() === pickedDate.getDate()));
  };

  //Prijs filter
  if(selectedPriceFilter == 'low'){
    filterResultaat.sort((a, b) => a.prijs_per_dag - b.prijs_per_dag);
  }else if(selectedPriceFilter == 'high'){
    filterResultaat.sort((a, b) => b.prijs_per_dag - a.prijs_per_dag);
  }

  //Zakelijke klant mag alleen autos zien
  if (role == "Zakelijkeklant" && isDisabled == false) {
    setIsDisabled(true);
    setSelectedCategory('Auto');
  }

  //Backoffice medewerkers moet voertuigen kunnen aanmaken en updaten (dit zijn de buttons om naar de goede paginas te gaan)
  if (role == "Backofficemedewerker"){
    const createVoertuig = document.getElementById('createVoertuig');
    if (createVoertuig) {
        createVoertuig.style.display = 'block';
      }

    const updateVoertuigButton = document.getElementById('updateVoertuigButton');
    if (updateVoertuigButton) {
      updateVoertuigButton.style.display = 'block';
      }
  }


  //Voertuig update buttons visible maken (handler) voor de backoffice medewerker
  const handleVoertuigUpdateButtons = () => {
    const updateVoertuigElements = document.querySelectorAll('.updateVoertuig');
    updateVoertuigElements.forEach((element) => {
      (element as HTMLElement).style.display = 'block';
    });
  }

  //handler om de gebruiker naar voertuig aanmaak pagina te sturen
  const handleNavigateCreateVoertuig = () => {
    if(role == "Backofficemedewerker"){
      navigate('/voertuigAanmaken');
    }
  }

  // handler om de gebruiker naar voertuig update pagina te sturen
  const handleNavigateUpdateVoertuig = (voertuigID: number) => {
    if(role == "Backofficemedewerker"){
      navigate('/voertuigUpdaten/' + voertuigID);
    }
  }

  return (
    <>
      <div className="overviewSection">
        <div className="headerFilter">
        <h1>Voertuigen overzicht</h1>
        <div>
        <label htmlFor="search">Zoek voor voertuig: </label>
        <input
        id="search"
        type="text"
        value={selectedNameFilter}
        onChange={(e) => setSelectedNameFilter(e.target.value)}
        placeholder="Type voertuig merk/naam"
        />
        </div>
        <div>
        <label htmlFor="categorieFilter">Soort voertuig: </label>
        <select id="categorieFilter" value={selectedCategory} 
          onChange={(e) => setSelectedCategory(e.target.value)} disabled = {isDisabled}>
          <option value="Alle">Alle</option>
          <option value="Auto">Auto</option>
          <option value="Caravan">Caravan</option>
          <option value="Camper">Camper</option>
        </select>
        </div>
        <div>
          <label>Filter beschikbare datums: 
          <DatePicker selectsRange={true}
            startDate={startDate}
            endDate={endDate}
            onChange={(update) => {
              setDateRange(update);
            }}
            isClearable={true}
          />
          </label>
        </div>
        <div id="createVoertuig">
          <button onClick={ handleNavigateCreateVoertuig }>+</button>
        </div>
        <div id="updateVoertuigButton">
          <button onClick={ handleVoertuigUpdateButtons }>Update</button>
        </div>
        </div>
        <table className="voertuigenTabel">
          <thead>
            <tr>
              <th>Categorie</th>
              <th>Voertuig</th>
              <th><label htmlFor="selectedPriceFilter" >Prijs per dag<select id="selectedPriceFilter" value={selectedPriceFilter} 
          onChange={(e) => setSelectedPriceFilter(e.target.value)}>
          <option value="none">-</option>
          <option value="low">Laag naar hoog</option>
          <option value="high">Hoog naar laag</option>
        </select></label></th>
              <th>Beschikbare datums</th>
            </tr>
          </thead>
          <tbody>
            {filterResultaat.map((voertuig, index) => (
              <tr key={index}>
                <td>{voertuig.voertuig_categorie}</td>
                <td>{voertuig.naam}</td>
                <td>{voertuig.prijs_per_dag}</td>
                <td className="datumButtonVoertuigen">
                  <DatePicker
                    selectsRange = {true}
                    startDate={verhuurAanvraagStartDate}
                    endDate={verhuurAanvraagEndDate}
                    onChange={([nieuweVerhuurAanvraagStartDate, nieuweVerhuurAanvraagEndDate]) => {
                      if(checkDates(nieuweVerhuurAanvraagStartDate, nieuweVerhuurAanvraagEndDate, voertuig.verhuur_perioden)){
                        alert("Not all the dates are available.");
                        setVerhuurAanvraagStartDate(null);
                        setVerhuurAanvraagEndDate(null);
                      }else {
                      setVerhuurAanvraagStartDate(nieuweVerhuurAanvraagStartDate);
                      setVerhuurAanvraagEndDate(nieuweVerhuurAanvraagEndDate);
                      if(verhuurAanvraagDateRange[1] !== null){
                        navigate('/verhuurAanvraag/' + voertuig.voertuigID + '/' + voertuig.naam + '/' + (new Date(verhuurAanvraagStartDate).toLocaleDateString("sv-SE")) + '/' + (new Date(verhuurAanvraagEndDate).toLocaleDateString("sv-SE")));
                      }
                    }
                    }
                  }
                    className="datepicker"
                    calendarStartDay={1}
                    excludeDates={getExcludedDates(voertuig.verhuur_perioden)}
                    customInput={<button>Beschikbare datums</button>}
                  />
                  <div className="updateVoertuig">
                    <button onClick={ () => handleNavigateUpdateVoertuig(voertuig.voertuigID) }>Update</button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}
