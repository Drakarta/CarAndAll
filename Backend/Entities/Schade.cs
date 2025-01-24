namespace Backend.Entities
{
    public class Schade
    {

        public int SchadeID { get; set; }
        public int VoertuigID { get; set; }
        public Voertuig Voertuig { get; set; } = null!;
        public string schade { get; set; }
    }
}