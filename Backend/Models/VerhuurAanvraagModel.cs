namespace Backend.Models
{
    //model voor het helpen bij het aanmaken van verhuuraanvragen met en zonder accessoires
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
        public double Verzekering_multiplier { get; set; }

        public int VoertuigID { get; set; }

        public List<AccessoireList>? Accessoires { get; set; }
    }

    public class AccessoireList
{
    public string AccessoireNaam { get; set; }
    public int Aantal { get; set; }
}
}
