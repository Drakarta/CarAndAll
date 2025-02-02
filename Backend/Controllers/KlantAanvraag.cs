using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KlantAanvraagController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public KlantAanvraagController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("GetKlantAanvragen")]
        public async Task<IActionResult> GetKlantAanvragen()
        {
            var accountEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (accountEmail == null)
            {
                return Unauthorized();
            }
            var account_id = await _context.Account
                .Where(a => a.Email == accountEmail.Value)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
            var aanvragen = await _context.VerhuurAanvragen
     .Where(a => a.Account.Id == account_id)
     .Include(a => a.Voertuig)
     .Include(a => a.VerhuurAanvraagAccessoires)
     .ThenInclude(vaa => vaa.Accessoire)
     .Select(a => new VerhuurAanvraagDto
     {
         AanvraagID = a.AanvraagID,
         Startdatum = a.Startdatum,
         Einddatum = a.Einddatum,
         Bestemming = a.Bestemming,
         Kilometers = a.Kilometers,
         Status = a.Status,
         Verzekering_multiplier = a.Verzekering_multiplier,
         Voertuig = new VoertuigDto
         {
             Merk = a.Voertuig.Merk,
             Type = a.Voertuig.Type,
             Kenteken = a.Voertuig.Kenteken,
             Kleur = a.Voertuig.Kleur,
             Prijs_per_dag = a.Voertuig.Prijs_per_dag
         },
         VerhuurAanvraagAccessoires = a.VerhuurAanvraagAccessoires
             .Select(vaa => new VerhuurAanvraagAccessoireDto
             {
                 AccessoireNaam = vaa.AccessoireNaam,
                 Aantal = vaa.Aantal,
                 ExtraPrijsPerDag = vaa.Accessoire.Extra_prijs_per_dag,
                 MaxAantal = vaa.Accessoire.Max_aantal
             }).ToList()
     }).ToListAsync();
            return Ok(aanvragen);
        }
    }
}
