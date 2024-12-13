using Backend.Interface;

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

        public Guid GetAccount_Id()
        {
            var guidString = "B27E650B-9C9F-424E-BACB-003C9EEB7A8E";
            Guid guid = Guid.Parse(guidString);
            return guid;
        }
    }
}