namespace Backend.Entities {
 public class Account
    {
        public Guid? Id { get; set; }
        public required string Email { get; set; }
        public required string Wachtwoord { get; set; }
        public string? Naam { get; set; }
        public string? Adres { get; set; }
        public string? TelefoonNummer { get; set; }
        public string? Rol { get; set; } // "Particuliere huurder", "Zakelijke huurder", "Wagenparkbeheerder"
        public ICollection<AccountBedrijf>? AccountBedrijven { get; set; }
    }
}