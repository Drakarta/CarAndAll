using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Entities;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KlantAanvraagController : ControllerBase {

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
    .Select(a => new VerhuurAanvraagDTO
    {
        AanvraagID = a.AanvraagID,
        Startdatum = a.Startdatum,
        Einddatum = a.Einddatum,
        Bestemming = a.Bestemming,
        Kilometers = a.Kilometers,
        Status = a.Status,
        Voertuig = new VoertuigDTO
        {
            Merk = a.Voertuig.Merk,
            Type = a.Voertuig.Type,
            Kenteken = a.Voertuig.Kenteken,
            Kleur = a.Voertuig.Kleur,
            Prijs_per_dag = a.Voertuig.Prijs_per_dag
        }
    }).ToListAsync();
            return Ok(aanvragen);
        }
    }
}
