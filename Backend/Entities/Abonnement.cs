
namespace Backend.Entities
{
    public class Abonnement
    {
        public int Id { get; set; }
        public string Naam { get; set; } = string.Empty;
        public double Prijs_multiplier { get; set; }
        public string Beschrijving { get; set; } = string.Empty;

        public int Max_medewerkers { get; set; }

        public ICollection<Bedrijf> Bedrijven { get; set; } = new List<Bedrijf>();

    }
}