import "../styles/abonnementen.css";

export default function Abonnementen() {
  const abonnementen = [
    { naam: "Pay as you go", prijs: "€100 / maand + €0.20 / km", beschrijving: "Onze pay-as-you-go bundel biedt procentuele kortingen aan op de huurbedragen die u zult betalen." },
    { naam: "Prepaid", prijs: "€100 / Dag", beschrijving: "De prepaid bundel zorgt ervoor dat u al gelijk kunt rijden en zich geen zorgen hoeft te maken over extra kosten!" },
  ];

  return (
    <div className="abonnementenSection">
      <h2>Abonnementen Overzicht</h2>
      <table className="abonnementenTabel">
        <thead>
          <tr>
            <th>Abonnement</th>
            <th>Beschrijving</th>
            <th>Prijs</th>
            <th>Acties</th>
          </tr>
        </thead>
        <tbody>
          {abonnementen.map((abonnement, index) => (
            <tr key={index}>
              <td>{abonnement.naam}</td>
              <td>{abonnement.beschrijving}</td>
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