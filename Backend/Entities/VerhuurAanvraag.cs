namespace Backend.Entities {
    public class VerhuurAanvraag {
        public int AanvraagID { get; set; }
        
        private DateTime _startdatum;
        public DateTime Startdatum {
            get { return _startdatum.Date; }
            set { _startdatum = value; }
        }
        
        private DateTime _einddatum;
        public DateTime Einddatum {
            get { return _einddatum.Date; }
            set { _einddatum = value; }
        }
        
        public string Bestemming { get; set; }
        public int Kilometers { get; set; }
        public int VoertuigID { get; set; }
        public Voertuig Voertuig { get; set; } = null!;
        public Account account { get; set; } = null!;
        public string Status { get; set; }
}
}