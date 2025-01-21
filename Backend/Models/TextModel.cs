namespace Backend.Models
{
    public class TextModelGet
    {
        public required string Type { get; set; }
    }

    public class TextModelUpdate
    {
        public required string Type { get; set; }
        public required string Content { get; set; }
    }
}
