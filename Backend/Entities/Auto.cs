namespace Backend.Entities {
    public class Auto : Voertuig
    {
        public int Aantal_deuren { get; set; }
        public bool Elektrisch { get; set; }
        public string voertuig_categorie = "Auto";
    }
}