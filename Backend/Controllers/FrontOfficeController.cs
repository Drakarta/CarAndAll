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
    public class FrontOfficeController : VerhuurAanvraagController
    {
        public FrontOfficeController(ApplicationDbContext context) : base(context)
        {
        }

        [Authorize(Policy = "FrontOffice")]
        [HttpPost("ChangeStatus")]
        public override async Task<IActionResult> ChangeStatus([FromBody] Request model) {
            return await base.ChangeStatus(model);
        }

        [Authorize(Policy = "FrontOffice")]
        [HttpGet("GetVerhuurAanvragenWithStatus")]
        public override async Task<IActionResult> GetVerhuurAanvragenWithStatus()
        {
            return await base.GetVerhuurAanvragenWithStatus();
        }
    }

}