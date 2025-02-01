namespace Backend.Entities
{
    public class VerhuurAanvraagAccessoire
    {
        public required int AanvraagID { get; set; }
        public required VerhuurAanvraag VerhuurAanvraag { get; set; }
        public required string AccessoireNaam { get; set; }

        public required Accessoire Accessoire { get; set; }
        public required int Aantal { get; set; }
    }
}