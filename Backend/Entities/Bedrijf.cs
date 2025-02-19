namespace Backend.Entities
{
    public class Bedrijf
    {

        public Guid Id { get; set; }
        public int? AbonnementId { get; set; }
        public Abonnement? abonnement { get; set; }
        public string? Domein { get; set; }
        public required Guid Eigenaar { get; set; }
        public required ICollection<BedrijfAccounts> BedrijfAccounts { get; set; }
        public required ICollection<BedrijfWagenparkbeheerders> BedrijfWagenparkbeheerders { get; set; }
        public ICollection<AbonnementAanvraag> AbonnementAanvragen { get; set; } = new List<AbonnementAanvraag>();
        public string? naam { get; set; }
    }
}