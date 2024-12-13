namespace Backend.Entities {
 public class Account
    {
      public Guid Id { get; set; }
        public required string Email { get; set; }
        public string? wachtwoord { get; set; }
        public string? Roles { get; set; }
        public ICollection<BedrijfAccounts>? BedrijfAccounts { get; set; } // Correct table name
        
    }
}