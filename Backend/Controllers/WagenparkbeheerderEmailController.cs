using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Interfaces;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WagenparkbeheerderEmailController : BaseEmailController
    {
        public WagenparkbeheerderEmailController(ApplicationDbContext context, IUserService userService, IEmailSender emailSender)
            : base(context, userService, emailSender)
        {
        }
    }
}
