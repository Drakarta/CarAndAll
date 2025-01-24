namespace Backend.Entities
{
    public class AbonnementAanvraag
    {
        public int Id { get; set; } // Primary Key
        public string Naam { get; set; } = string.Empty; // Required Name
        public string Beschrijving { get; set; } = string.Empty; // Required Description
        public double PrijsMultiplier { get; set; } // Required Price Multiplier
        public int MaxMedewerkers { get; set; } // Maximum Allowed Employees
        public Guid BedrijfId { get; set; } // Foreign Key
        public Bedrijf Bedrijf { get; set; } = null!; // Navigation Property
        public string Status { get; set; } = "In behandeling"; // Default Status
    }
}
