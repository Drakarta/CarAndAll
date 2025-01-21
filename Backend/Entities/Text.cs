using Microsoft.EntityFrameworkCore;

namespace Backend.Entities
{
    [Keyless]
    public class Text
    {
        public required string type { get; set; }
        public required string content { get; set; }
    }
}