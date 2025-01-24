namespace Backend.Entities
{
    public class LoginRegisterModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Naam { get; set; }
        public string? Role { get; set; } // New property for user role
        public string? Address { get; set; } // New property for address
        public string? PhoneNumber { get; set; } // New property for phone number
    }
}