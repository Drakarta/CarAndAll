namespace Backend.Entities {
    //Caravan class die alle variablen van voertuig overneemt
    public class Caravan : Voertuig
    {
        public int Aantal_slaapplekken { get; set; }
        public int Gewicht_kg { get; set; }
        public string voertuig_categorie { get; } = "Caravan";
    }
}