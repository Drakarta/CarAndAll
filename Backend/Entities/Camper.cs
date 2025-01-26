namespace Backend.Entities {
    public class Camper : Voertuig
    {
        public int Aantal_slaapplekken { get; set; }
        public bool Elektrisch { get; set; }
        public string voertuig_categorie { get; } = "Camper";
    }
}