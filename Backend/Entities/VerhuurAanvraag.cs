namespace Backend.Entities {
    public class VerhuurAanvraag
    {
        public int AanvraagID { get; }
        public DateTime Startdatum { get {
            return Startdatum.Date;
        } set {
            Startdatum = value;
        }}
        public DateTime Einddatum { get {
            return Einddatum.Date;
        } set {
            Einddatum = value;
        }}
        public string Bestemming { get; set; }
        public int Kilometers{ get; set; }
        public int VoertuigID { get; set; }
        public Voertuig Voertuig { get; set; } = null!;
    }
}