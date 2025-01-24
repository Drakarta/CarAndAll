namespace Backend.Models
{
    public class EmailModelAdd
    {
        public required string Email { get; set; }
        public required string Role { get; set; }
    }

    public class EmailModelRemove
    {
        public required string Email { get; set; }

    }
}
