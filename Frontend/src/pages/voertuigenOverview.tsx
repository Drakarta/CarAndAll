import { useEffect, useState, useRef } from "react";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import "../styles/voertuigenOverview.css";

export default function VoertuigenOverview() {
  const [Voertuigen, setVoertuigen] = useState([]);
  const [selectedCategory, setSelectedCategory] = useState("Alle");
  const [selectedPriceFilter, setSelectedPriceFilter] = useState("none");
  const [selectedNameFilter, setSelectedNameFilter] = useState("");
  const [dateRange, setDateRange] = useState([null, null]);
  const [startDate, endDate] = dateRange;

  useEffect(() => {
    const fetchVoertuigen = async () => {
      try {
        const response = await fetch(`${import.meta.env.VITE_REACT_APP_VOERTUIGEN_API_URL}`, {
          headers: {
              'Authorization': `Bearer ${import.meta.env.VITE_REACT_APP_API_KEY}`
          }
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

  const categoryFilter = selectedCategory === "Alle" || voertuig.categorie === selectedCategory;

  if (!startDate || !endDate) {
    return categoryFilter;
  }

  const filterStartdatum = new Date(startDate);
  const filterEinddatum = new Date(endDate);

  const verhuurAanvraagOverlap = voertuig.verhuur_perioden.some((periode) => {
    const verhuurStart = new Date(periode.verhuur_start);
    const verhuurEnd = new Date(periode.verhuur_eind);

    return !(filterEinddatum < verhuurStart || filterStartdatum > verhuurEnd);
  });

  return categoryFilter && !verhuurAanvraagOverlap;
}).filter((voertuig) => voertuig.naam.toLowerCase()
.includes(selectedNameFilter.toLowerCase())).sort((a, b) => a.naam.localeCompare(b.naam));

  //Het ophalen van de datums die niet open zijn
  const getExcludedDates = (verhuurPerioden) => {
    const excludedDates = [];
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

  if(selectedPriceFilter == 'low'){
    filterResultaat.sort((a, b) => a.prijs_per_dag - b.prijs_per_dag);
  }else if(selectedPriceFilter == 'high'){
    filterResultaat.sort((a, b) => b.prijs_per_dag - a.prijs_per_dag);
  }

    // filterResultaat
  
  return (
    <>
      <div className="overviewSection">
        <div className="headerFilter">
        <h2>Voertuigen overzicht</h2>
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
          onChange={(e) => setSelectedCategory(e.target.value)}>
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
        </div>
        <table className="voertuigenTabel">
          <thead>
            <tr>
              <th>Categorie</th>
              <th>Voertuig</th>
              <th>Prijs per dag<select id="selectedPriceFilter" value={selectedPriceFilter} 
          onChange={(e) => setSelectedPriceFilter(e.target.value)}>
          <option value="none">-</option>
          <option value="low">Laag naar hoog</option>
          <option value="high">Hoog naar laag</option>
        </select></th>
              <th>Beschikbare datums</th>
            </tr>
          </thead>
          <tbody>
            {filterResultaat.map((voertuig, index) => (
              <tr key={index}>
                <td>{voertuig.categorie}</td>
                <td>{voertuig.naam}</td>
                <td>{voertuig.prijs_per_dag}</td>
                <td className="datumButtonVoertuigen">
                  <DatePicker
                    className="datepicker"
                    calendarStartDay={1}
                    excludeDates={getExcludedDates(voertuig.verhuur_perioden)}
                    customInput={<button>Beschikbare datums</button>}
                  />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}
