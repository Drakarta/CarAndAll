using System.ComponentModel.DataAnnotations.Schema;
namespace Backend.Entities {
    public class VerhuurAanvraag
    {
        public int AanvraagID { get; }
        private DateTime _startdatum;
        public DateTime Startdatum
        {
            get => _startdatum.Date;
            set => _startdatum = value;
        }
        private DateTime _einddatum;
        public DateTime Einddatum
        {
            get => _einddatum.Date;
            set => _einddatum = value;
        }
        public string Bestemming { get; set; }
        public int Kilometers{ get; set; }
        public int VoertuigID { get; set; }
        public Voertuig Voertuig { get; set; } = null!;

        [Column("AccountId")]
        public Guid GebruikerID { get; set; }
        public Account Account { get; set; } = null!;
        public string Status { get; set; }

    }
}