import { useEffect, useState } from "react";
import "../styles/voertuigenOverview.css";

export default function VoertuigenOverview() {
  const [Voertuigen, setVoertuigen] = useState([]);
  const [selectedCategory, setSelectedCategory] = useState("All");

  useEffect(() => {
    const fetchVoertuigen = async () => {
      try {
        const response = await fetch("http://localhost:5016/api/Voertuig/Voertuigen");
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
    return selectedCategory === "Alle" || voertuig.categorie === selectedCategory;
  });

  return (
    <>
      <div className="overviewSection">
        <h2>Voertuigen overzicht</h2>
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
        <table className="voertuigenTabel">
          <thead>
            <tr>
              <th>Categorie</th>
              <th>Voertuig</th>
              <th>Prijs</th>
              <th>Beschikbare datums</th>
            </tr>
          </thead>
          <tbody>
            {filterResultaat.map((voertuig, index) => (
              <tr key={index}>
                <td>{voertuig.categorie}</td>
                <td>{voertuig.naam}</td>
                <td>TBD</td>
                <td>
                  <button>Beschikbare datums</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}
