using Newtonsoft.Json;

namespace Backend.Models
{
    public class EmailModel
    {
        [JsonProperty("email")]
        public required string Email { get; set; }
    }
}
