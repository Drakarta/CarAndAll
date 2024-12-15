import { useState } from "react";
import "../styles/abonnementen.css";

export default function Abonnementen() {
  // Define state for subscriptions and selected category
  const [selectedCategory, setSelectedCategory] = useState("Alle");

  // Mock subscription options
  const abonnementen = [
    { categorie: "Pay as you go", naam: "Pay as you go", prijs: "€100 / maand", beschrijving: "Onze pay-as-you-go bundel biedt procentuele kortingen aan op de huurbedragen die u zult betalen." },
    { categorie: "Prepaid", naam: "Prepaid", prijs: "€100 / Dag", beschrijving: "De prepaid bundel zorgt ervoor dat u al gelijk kunt rijden en zich geen zorgen hoeft te maken over extra kosten!" },
  ];

  // Filter subscriptions based on the selected category
  const filterResultaat = abonnementen.filter((abonnement) => {
    return selectedCategory === "Alle" || abonnement.categorie === selectedCategory;
  });

  return (
    <div className="abonnementenSection">
      <h2>Abonnementen Overzicht</h2>
      <div>
        <label htmlFor="categorieFilter">Abonnementstype: </label>
        <select
          id="categorieFilter"
          value={selectedCategory}
          onChange={(e) => setSelectedCategory(e.target.value)}
        >
          <option value="Alle">Alle</option>
          <option value="Basis">Basis</option>
          <option value="Premium">Premium</option>
        </select>
      </div>
      <table className="abonnementenTabel">
        <thead>
          <tr>
            <th>Categorie</th>
            <th>Abonnement</th>
            <th>Prijs</th>
            <th>Acties</th>
          </tr>
        </thead>
        <tbody>
          {filterResultaat.map((abonnement, index) => (
            <tr key={index}>
              <td>{abonnement.categorie}</td>
              <td>{abonnement.naam}</td>
              <td>{abonnement.prijs}</td>
              <td>
                <button onClick={() => alert(`Gekozen abonnement: ${abonnement.naam}`)}>
                  Kies
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
