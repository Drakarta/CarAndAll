namespace Backend.Entities {
    public class Caravan : Voertuig
    {
        public int Aantal_slaapplekken { get; set; }
        public int Gewicht_kg { get; set; }
        public string voertuig_categorie = "Caravan";
    }
}