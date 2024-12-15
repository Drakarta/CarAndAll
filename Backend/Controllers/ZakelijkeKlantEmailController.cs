using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Interfaces;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZakelijkeKlantEmailController : BaseEmailController
    {
        public ZakelijkeKlantEmailController(ApplicationDbContext context, IUserService userService, IEmailSender emailSender)
            : base(context, userService, emailSender)
        {
        }
    }
}
