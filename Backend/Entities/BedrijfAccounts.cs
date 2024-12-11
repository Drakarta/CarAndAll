namespace Backend.Entities
{
    public class BedrijfAccounts
    {
        public Guid account_id { get; set; }
        public required Account Account { get; set; }
        public Guid bedrijf_id { get; set; }
        public required Bedrijf Bedrijf { get; set; }
    }
}