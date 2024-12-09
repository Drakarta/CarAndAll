namespace Backend.Entities
{
    public class AccountBedrijf
    {
        public int account_id { get; set; }
        public required Account Account { get; set; }
        public int bedrijf_id { get; set; }
        public required Bedrijf Bedrijf { get; set; }
    }
}