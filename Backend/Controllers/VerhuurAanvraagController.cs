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
    public class VerhuurAanvraagController : ControllerBase
    {
          protected readonly ApplicationDbContext _context;

        public VerhuurAanvraagController(ApplicationDbContext context)
        {
            _context = context;
        }

        // [Authorize(Policy = "ParticuliereHuurder")]
        [HttpPost("createVerhuurAanvraag")]
        public async Task<IActionResult> CreateVerhuurAanvraag([FromBody] VerhuurAanvraagModel model)
        {
            try
            {
                var accountEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var account_id = await _context.Account
                    .Where(a => a.Email == accountEmail.Value)
                    .Select(a => a.Id)
                    .FirstOrDefaultAsync();

                var account = await _context.Account.FindAsync(account_id);

                var voertuig = await _context.Voertuigen.FindAsync(model.VoertuigID);                

                var verhuurAanvraag = new VerhuurAanvraag
                {
                    Startdatum = model.Startdatum,
                    Einddatum = model.Einddatum,
                    Bestemming = model.Bestemming,
                    Kilometers = model.Kilometers,
                    VoertuigID = model.VoertuigID,
                    Voertuig = voertuig,
                    GebruikerID = account_id,
                    Account = account,
                    Status = "In behandeling"
                };

                _context.VerhuurAanvragen.Add(verhuurAanvraag);
                await _context.SaveChangesAsync();

                var succes = new {
                    message = "Verhuur aanvraag succesvol ingediend.",
                    statusCode = 200
                };
                return Ok (succes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}