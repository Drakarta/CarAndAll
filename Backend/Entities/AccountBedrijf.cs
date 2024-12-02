namespace CarAndAll
{
    public class AccountBedrijf
    {
        public int account_id { get; set; }
        public Account Account { get; set; }
        
        public int bedrijf_id { get; set; }
        public Bedrijf Bedrijf { get; set; }
    }
}