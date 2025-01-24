using Microsoft.EntityFrameworkCore;

namespace Backend.Entities
{
    public class Text
    {
        public required string Type { get; set; } // primary key
        public required string Content { get; set; }
    }
}