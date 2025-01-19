namespace Backend.Entities
{
    public class BedrijfWagenparkbeheerders
    {
        public Guid account_id { get; set; }
        public required Account Account { get; set; }
        public Guid bedrijf_id { get; set; }
        public required Bedrijf Bedrijf { get; set; }
    }
}