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
using Microsoft.IdentityModel.Tokens;


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


        //Methode voor het ophalen van de verhuuraanvraag data uit de frontend en daarna opslaan in de database.
        [Authorize(Policy = "ParticuliereZakelijkeHuurder")]
        [HttpPost("createVerhuurAanvraag")]
        public async Task<IActionResult> CreateVerhuurAanvraag([FromBody] VerhuurAanvraagModel model)
        {
            try
            {
                 if (model.Bestemming.IsNullOrEmpty())
                {
                    var errorDetails = new {
                        message = "The 'Bestemming'input is empty",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }
                if (model.Kilometers == 0)
                {
                    var errorDetails = new {
                        message = "The 'Kilometers' input is empty",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }
                
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