using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using Backend.Data;
using Backend.Entities;
using Backend.Interfaces;
using Backend.Helpers;
using Backend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WagenparkbeheerderEmailController : BaseEmailController
    {
        public WagenparkbeheerderEmailController(ApplicationDbContext context, IEmailSender emailSender)
            : base(context, emailSender)
        {
        }

        [Authorize(Policy="Wagenparkbeheerder")]
        [HttpGet("emails")]
        public override async Task<IActionResult> GetEmails()
        {
            Console.WriteLine("Authorization check for Wagenparkbeheerder policy");
            return await base.GetEmails();
        }

        [Authorize(Policy="Wagenparkbeheerder")]
        [HttpPost("addUserToCompany")]
        public override async Task<IActionResult> AddUserToCompany([FromBody] EmailModel model)
        {
            return await base.AddUserToCompany(model);
        }

        [Authorize(Policy="Wagenparkbeheerder")]
        [HttpPost("removeUserFromCompany")]
        public override async Task<IActionResult> RemoveUserFromCompany([FromBody] EmailModel model)
        {
            return await base.RemoveUserFromCompany(model);
        }
    }
}
