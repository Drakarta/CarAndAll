namespace Backend.Entities
{
    public class VoertuigCategorie
    {
        public string Categorie { get; set; }
        public ICollection<Voertuig> Voertuigen { get; set; }
    }
}