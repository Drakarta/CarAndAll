using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Entities;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


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
                    var errorDetails = new
                    {
                        message = "The 'Bestemming'input is empty",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }
                if (model.Kilometers == 0)
                {
                    var errorDetails = new
                    {
                        message = "The 'Kilometers' input is empty",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }
                if (model.Verzekering_multiplier == 0)
                {
                    var errorDetails = new
                    {
                        message = "The 'Verzekering' input cannot be 0",
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
                    Verzekering_multiplier = model.Verzekering_multiplier,
                    VoertuigID = model.VoertuigID,
                    Voertuig = voertuig,
                    Account = account,
                    Status = "In behandeling"
                };
                _context.VerhuurAanvragen.Add(verhuurAanvraag);
                //verhuuraanvraag extra accessoires verwerking
                if (model.Accessoires != null && model.Accessoires.Any())
                {
                    foreach(AccessoireList accessoireList in model.Accessoires){
                        var bestaandAccessoire = await _context.Accessoires.FindAsync(accessoireList.AccessoireNaam);
                        if(accessoireList.Aantal > 0 && bestaandAccessoire?.AccessoireNaam == accessoireList.AccessoireNaam){
                            if(accessoireList.Aantal <= bestaandAccessoire.Max_aantal){
                            var tempVerhuurAanvraagAccessoire = new VerhuurAanvraagAccessoire {
                                AanvraagID = verhuurAanvraag.AanvraagID,
                                VerhuurAanvraag = verhuurAanvraag,
                                AccessoireNaam = accessoireList.AccessoireNaam,
                                Accessoire = bestaandAccessoire,
                                Aantal = accessoireList.Aantal
                            };
                            _context.VerhuurAanvraagAccessoires.Add(tempVerhuurAanvraagAccessoire);
                            }else {
                                _context.VerhuurAanvragen.Remove(verhuurAanvraag);
                                var errorDetails = new
                                {
                                    message = "Over the max limit on 1 or more accessoires",
                                    statusCode = 400
                                };
                                return BadRequest(errorDetails);
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();

                var succes = new
                {
                    message = "Verhuur aanvraag succesvol ingediend.",
                    statusCode = 200
                };
                return Ok(succes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //Accessoires ophalen zodat de gebruiker accessoires kan huren bij zijn/haar auto.
        [Authorize(Policy = "ParticuliereZakelijkeHuurder")]
        [HttpGet("GetAccessoires")]
        public async Task<IActionResult> GetAccessoires()
        {
            try
            {
            var accessoires = await _context.Accessoires.Select(a => new 
                                    {
                                        accessoireNaam = a.AccessoireNaam,
                                        extra_prijs_per_dag = a.Extra_prijs_per_dag,
                                        max_aantal = a.Max_aantal,
                                    })
                                    .ToListAsync();

                return Ok(accessoires);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}