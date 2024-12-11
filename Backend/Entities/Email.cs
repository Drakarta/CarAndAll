namespace Backend.Entities
{
    public class Email
    {
        public required string Id { get; set; }
        public required string Address { get; set; }

        public int account_id { get; set; }
        public Account? Account { get; set; } 

        public int bedrijf_id { get; set; }
        public Bedrijf? Bedrijf { get; set; }
    }
}