namespace Backend.Entities
{
    public class AccountBedrijf
    {
        public Guid Account_id { get; set; }
        public required Account Account { get; set; }
        public int Bedrijf_id { get; set; }
        public required Bedrijf Bedrijf { get; set; }
    }
}