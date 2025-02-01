namespace Backend.Entities
{
    public class Accessoire
    {
        public required string AccessoireNaam { get; set; }
        public int Extra_prijs_per_dag { get; set; }
        public int Max_aantal { get; set; }
        public ICollection<VerhuurAanvraagAccessoire> VerhuurAanvraagAccessoires { get; set;}
    }
}