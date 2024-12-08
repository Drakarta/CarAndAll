namespace Backend.Entities
{
    public class Email
    {
        public int Id { get; set; }
        public required string Address { get; set; }

        // Foreign keys to link emails to accounts and companies
        public int account_id { get; set; }
        public Account? Account { get; set; }  // Navigation property to Account

        public int bedrijf_id { get; set; }
        public Bedrijf? Bedrijf { get; set; }
    }
}