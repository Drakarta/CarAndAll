namespace Backend.Entities {
 public class Account
    {
        public required Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Wachtwoord { get; set; }
        public string? Naam { get; set; }
        public string? Adres { get; set; }
        public string? TelefoonNummer { get; set; }
        // public string? Roles { get; set; }
        public ICollection<AccountBedrijf>? AccountBedrijven { get; set; } // Update this line
    }
}