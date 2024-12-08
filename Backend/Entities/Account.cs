namespace Backend.Entities {
 public class Account
    {
      public required int Id { get; set; }
        public required string Email { get; set; }
        // Other properties like username, password, etc.
        public required ICollection<AccountBedrijf> AccountBedrijven { get; set; } // Update this line
    }
}