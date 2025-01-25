namespace Backend.Models
{
    public class CreateVoertuigModel
    {
        public required string Merk { get; set;}
        public required string Type { get; set; }
        public required string Kenteken { get; set; }
        public required string Kleur { get; set; }
        public required string Aanschafjaar{ get; set; }
        public required double Prijs_per_dag { get; set; }
        public int? Aantal_deuren { get; set; }
        public bool? Elektrisch { get; set; }
        public int? Aantal_slaapplekken { get; set; }
        public int? Gewicht_kg { get; set; }
        public required string voertuig_categorie { get; set; }
        public required string Status { get; set; }
    }
}
