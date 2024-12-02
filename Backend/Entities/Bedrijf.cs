namespace CarAndAll
{
public class Bedrijf
{

 public int Id { get; set; }
        public required string Name { get; set; }
        
        // Navigation property to Account_Bedrijf
        public ICollection<AccountBedrijf> AccountBedrijven { get; set; } // Add this line
}
}