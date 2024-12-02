namespace CarAndAll { 
 
 public class Account
    {
      public int Id { get; set; }
        public required string Email { get; set; }
        // Other properties like username, password, etc.
        public ICollection<AccountBedrijf> AccountBedrijven { get; set; } // Update this line
    }
}