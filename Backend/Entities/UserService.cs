using Backend.Interface;

namespace Backend.Entities
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        // Add any additional properties you want to store about the user
        public int Account_Id { get; set; }
    }

    public class UserService  : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public int GetAccount_Id()
        {
            // Hardcode the account ID to 1
            return 1;
        }
    }
}