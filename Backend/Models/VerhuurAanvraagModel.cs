namespace Backend.Models
{
    public class VerhuurAanvraagModel
    {
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
        public int Kilometers { get; set; }

        public int VoertuigID { get; set; }
    }
}
