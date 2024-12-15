using Backend.Interfaces;

namespace Backend.Entities
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        public Guid Account_Id { get; set; }
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

        public Guid GetAccount_Id(string token)
        {
            if (Guid.TryParse(token, out Guid guid))
            {
                return guid;
            }
            throw new ArgumentException("Invalid token format");
        }
    }
}