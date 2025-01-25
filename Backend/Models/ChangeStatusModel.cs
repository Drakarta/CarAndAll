
namespace Backend.Models 
    {
    public class ChangeStatusModel
    {
        public int AanvraagID { get; set; }
        public required string Status { get; set; } = string.Empty;

    }
    }