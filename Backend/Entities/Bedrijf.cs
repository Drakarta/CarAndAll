namespace Backend.Entities
{
        public class Bedrijf
        {

                public int Id { get; set; }
                // public string? Name { get; set; }
                public required int Abbonement { get; set; }
                public string? Domein { get; set; }
                public required int Eigenaar {get; set; }
                public required ICollection<AccountBedrijf> AccountBedrijven { get; set; } // Add this line
        }
}