namespace Backend.Entities
{
        public class Bedrijf
        {

                public Guid Id { get; set; }
                public required string Abbonement { get; set; }
                public string? Domein { get; set; }
                public required Guid Eigenaar {get; set; }
                public required ICollection<BedrijfAccounts> BedrijfAccounts { get; set; } // Add this line
                public string? naam { get; set; }
        }
}