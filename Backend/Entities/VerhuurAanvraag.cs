using Microsoft.EntityFrameworkCore;

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
        public Account Account { get; set; } = null!;
        public string Status { get; set; }
}

[Keyless]
public class VerhuurAanvraagDTO
{
    public int AanvraagID { get; set; }
    public DateTime Startdatum { get; set; }
    public DateTime Einddatum { get; set; }
    public string Bestemming { get; set; }
    public int Kilometers { get; set; }
    public VoertuigDTO Voertuig { get; set; }
    public string Status { get; set; }
    public Guid accountID { get; set; }
}

[Keyless]
public class VerhuurAanvraagDetailsDTO
{
    public int AanvraagID { get; set; }
    public DateTime Startdatum { get; set; }
    public DateTime Einddatum { get; set; }
    public string Bestemming { get; set; }
    public int Kilometers { get; set; }
    public VoertuigDTO Voertuig { get; set; }
    public string Status { get; set; }
    public Guid AccountID { get; set; }
    public string AccountEmail { get; set; }
}
}