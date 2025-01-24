namespace Backend.Models {
 public class RegisterModel
    {
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }
    }
    public class LoginModel
    {
    public required string Email { get; set; }
    public required string Password { get; set; }
    }

    public class LoginRegisterModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Role { get; set; }
        public string? Naam { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
