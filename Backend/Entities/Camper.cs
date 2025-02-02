namespace Backend.Entities {
    //Camper class die alle variablen van voertuig overneemt
    public class Camper : Voertuig
    {
        public int Aantal_slaapplekken { get; set; }
        public bool Elektrisch { get; set; }
        public string voertuig_categorie { get; } = "Camper";
    }
}