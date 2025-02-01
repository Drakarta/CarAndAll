namespace Backend.Entities {
    //Auto class die alle variablen van voertuig overneemt
    public class Auto : Voertuig
    {
        public int Aantal_deuren { get; set; }
        public bool Elektrisch { get; set; }
        public string voertuig_categorie { get; } = "Auto";
    }
}