namespace Backend.Entities {
 public class Account
    {
      public Guid Id { get; set; }
        public required string Email { get; set; }
        public string wachtwoord { get; set; }
        // Other properties like username, password, etc.
        public ICollection<AccountBedrijf>? AccountBedrijven { get; set; } // Correct table name
    }
}