namespace Backend.Entities {
    public class  Voertuig
    {
        public int VoertuigID { get; }
        public string Merk { get; set;}
        public string Type { get; set; }
        public string Kenteken { get; set; }
        public string Kleur { get; set; }
        public string Aanschafjaar{ get; set; }
        public string Status { get; set; }
        public double Prijs_per_dag { get; set; }
        public DateTime? Deleted_on { get; set; } = null;
        public string voertuig_categorie { get; set; }
        public ICollection<VerhuurAanvraag> VerhuurAanvragen { get; set;}
    }

    public class VoertuigDTO
{
    public string Merk { get; set; }
    public string Type { get; set; }
    public string Kenteken { get; set; }
    public string Kleur { get; set; }
    public double Prijs_per_dag { get; set; }
}
}