using Microsoft.EntityFrameworkCore;

namespace Backend.Entities
{
    //Verhuuraanvraag class met alle verhuuraanvraag variablen
    public class VerhuurAanvraag
    {
        public int AanvraagID { get; set; }

        private DateTime _startdatum;
        public DateTime Startdatum
        {
            get { return _startdatum.Date; }
            set { _startdatum = value; }
        }

        private DateTime _einddatum;
        public DateTime Einddatum
        {
            get { return _einddatum.Date; }
            set { _einddatum = value; }
        }

        public string Bestemming { get; set; }
        public int Kilometers { get; set; }
        public int VoertuigID { get; set; }
        public Voertuig Voertuig { get; set; } = null!;
        public Account Account { get; set; } = null!;
        public string Status { get; set; }
        public double Verzekering_multiplier { get; set; } = 1.0;
        public ICollection<VerhuurAanvraagAccessoire> VerhuurAanvraagAccessoires { get; set;}
    }

    [Keyless]
    public class VerhuurAanvraagDto
    {
        public int AanvraagID { get; set; }
        public DateTime Startdatum { get; set; }
        public DateTime Einddatum { get; set; }
        public required string Bestemming { get; set; }
        public int Kilometers { get; set; }
        public required VoertuigDto Voertuig { get; set; }
        public required string Status { get; set; }
        public double Verzekering_multiplier { get; set; }
        public Guid accountID { get; set; }
        public ICollection<VerhuurAanvraagAccessoireDto> VerhuurAanvraagAccessoires { get; set; }
    }

    [Keyless]
    public class VerhuurAanvraagDetailsDto
    {
        public int AanvraagID { get; set; }
        public DateTime Startdatum { get; set; }
        public DateTime Einddatum { get; set; }
        public required string Bestemming { get; set; }
        public int Kilometers { get; set; }
        public required VoertuigDto Voertuig { get; set; }
        public required string Status { get; set; }
        public Guid AccountID { get; set; }
        public required string AccountEmail { get; set; }
        public string AccountNaam { get; set; } = "Unknown";
    }
}