namespace Backend.Entities
{
    //Tussen table class tussen verhuuraanvraag en accessoires
    public class VerhuurAanvraagAccessoire
    {
        public required int AanvraagID { get; set; }
        public required VerhuurAanvraag VerhuurAanvraag { get; set; }
        public required string AccessoireNaam { get; set; }

        public required Accessoire Accessoire { get; set; }
        public required int Aantal { get; set; }
    }

    public class VerhuurAanvraagAccessoireDto
    {
        public required string AccessoireNaam { get; set; }
        public required int Aantal { get; set; }
        public required int ExtraPrijsPerDag { get; set; }
        public required int MaxAantal { get; set; }
    }
}