using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AbonnementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AbonnementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Abonnement
        [Authorize(Policy = "Wagenparkbeheerder")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Abonnement>>> GetAbonnementen()
        {
            var abonnementen = await _context.Abonnement.ToListAsync();
            return Ok(abonnementen);
        }

        // GET: api/Abonnement/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Abonnement>> GetAbonnementById(int id)
        {
            var abonnement = await _context.Abonnement.FindAsync(id);
            if (abonnement == null)
            {
                return NotFound("Abonnement niet gevonden");
            }
            return Ok(abonnement);
        }

        // GET: api/Abonnement/currentBedrijf
        [Authorize (Policy = "Wagenparkbeheerder")]
        [HttpGet("currentBedrijf")]
        public async Task<IActionResult> GetCurrentBedrijf()
        {
            try
            {
                // Haal de e-mail en rol van de huidige gebruiker op basis van de ingelogde sessie of claims
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

                if (emailClaim == null || roleClaim == null)
                {
                    Console.WriteLine("Gebruiker is niet geauthenticeerd of rol is niet beschikbaar.");
                    return NotFound("Gebruiker is niet geauthenticeerd of rol is niet beschikbaar.");
                }

                var email = emailClaim.Value;
                var role = roleClaim.Value;

                // Log de opgehaalde e-mail en rol
                Console.WriteLine($"Opgehaalde e-mail: {email}");
                Console.WriteLine($"Opgehaalde rol: {role}");

                // Bevestig dat de rol 'Wagenparkbeheerder' is
                if (role != "Wagenparkbeheerder")
                {
                    Console.WriteLine("Gebruiker heeft niet de vereiste rol.");
                    return Unauthorized(new { message = "Gebruiker heeft niet de vereiste rol." });
                }

                // Haal de 'BedrijfWagenparkbeheerders' op die zijn gekoppeld aan de e-mail van de ingelogde gebruiker
                var bedrijfWagenparkbeheerder = await _context.BedrijfWagenparkbeheerders
                    .Include(bw => bw.Bedrijf)
                    .FirstOrDefaultAsync(bw => bw.Account.Email == email);

                if (bedrijfWagenparkbeheerder == null)
                {
                    Console.WriteLine("Geen gekoppeld Bedrijf gevonden voor de huidige gebruiker.");
                    return NotFound(new { message = "Geen gekoppeld Bedrijf gevonden voor de huidige gebruiker." });
                }

                var bedrijfId = bedrijfWagenparkbeheerder.bedrijf_id;

                // Log de opgehaalde bedrijfId
                Console.WriteLine($"Opgehaalde bedrijfId: {bedrijfId}");

                // Retourneer de 'Bedrijf' Id
                return Ok(new { bedrijfId = bedrijfId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Interne serverfout: {ex.Message}");
                return StatusCode(500, new { message = $"Interne serverfout: {ex.Message}" });
            }
        }

        // POST: api/Abonnement
        [HttpPost]
        public async Task<ActionResult<Abonnement>> CreateAbonnement(Abonnement newAbonnement)
        {
            await _context.Abonnement.AddAsync(newAbonnement);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAbonnementById), new { id = newAbonnement.Id }, newAbonnement);
        }

        // PUT: api/Abonnement/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAbonnement(int id, Abonnement updatedAbonnement)
        {
            var abonnement = await _context.Abonnement.FindAsync(id);
            if (abonnement == null)
            {
                return NotFound("Abonnement niet gevonden");
            }

            abonnement.Naam = updatedAbonnement.Naam;
            abonnement.Prijs_multiplier = updatedAbonnement.Prijs_multiplier;
            abonnement.Beschrijving = updatedAbonnement.Beschrijving;
            abonnement.Max_medewerkers = updatedAbonnement.Max_medewerkers;

            _context.Abonnement.Update(abonnement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Abonnement/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAbonnement(int id)
        {
            var abonnement = await _context.Abonnement.FindAsync(id);
            if (abonnement == null)
            {
                return NotFound("Abonnement niet gevonden");
            }

            _context.Abonnement.Remove(abonnement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Abonnement/select
        [Authorize]
        [HttpPost("select")]
        public async Task<IActionResult> SelectAbonnement([FromBody] JsonElement data)
        {
            try
            {
                // Zorg ervoor dat `abonnementId` is opgegeven
                if (!data.TryGetProperty("abonnementId", out var abonnementIdProp))
                {
                    return BadRequest(new { message = "Ontbrekend vereist veld 'abonnementId'.", statusCode = 400 });
                }

                int abonnementId = abonnementIdProp.GetInt32();

                // Identificeer de huidige gebruiker en hun bedrijf (ervan uitgaande dat sessie of authenticatie gebruikersinformatie biedt)
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (emailClaim == null)
                {
                    return Unauthorized(new { message = "Gebruiker is niet geauthenticeerd.", statusCode = 401 });
                }

                var email = emailClaim.Value;

                // Haal de 'BedrijfWagenparkbeheerders' op die zijn gekoppeld aan de e-mail van de ingelogde gebruiker
                var bedrijfWagenparkbeheerder = await _context.BedrijfWagenparkbeheerders
                    .Include(bw => bw.Bedrijf)
                    .FirstOrDefaultAsync(bw => bw.Account.Email == email);

                if (bedrijfWagenparkbeheerder == null)
                {
                    return NotFound(new { message = "Bedrijf niet gevonden voor de huidige gebruiker.", statusCode = 404 });
                }

                var bedrijf = bedrijfWagenparkbeheerder.Bedrijf;

                // Zoek het opgegeven abonnement
                var abonnement = await _context.Abonnement.FindAsync(abonnementId);
                if (abonnement == null)
                {
                    return NotFound(new { message = "Abonnement niet gevonden.", statusCode = 404 });
                }

                // Werk het abonnement van het bedrijf bij
                bedrijf.AbonnementId = abonnement.Id;
                _context.Bedrijf.Update(bedrijf);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Abonnement {abonnement.Naam} succesvol geselecteerd voor Bedrijf {bedrijf.naam}", statusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Interne serverfout: {ex.Message}", statusCode = 500 });
            }
        }
    }
}