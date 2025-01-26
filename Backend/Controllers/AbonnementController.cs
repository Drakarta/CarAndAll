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
                return NotFound("Abonnement not found");
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
                // Retrieve the current user's email and role based on the logged-in session or claims
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

                if (emailClaim == null || roleClaim == null)
                {
                    Console.WriteLine("User is not authenticated or role is not available.");
                    return NotFound("User is not authenticated or role is not available.");
                }

                var email = emailClaim.Value;
                var role = roleClaim.Value;

                // Log the retrieved email and role
                Console.WriteLine($"Retrieved email: {email}");
                Console.WriteLine($"Retrieved role: {role}");

                // Confirm that the role is 'Wagenparkbeheerder'
                if (role != "Wagenparkbeheerder")
                {
                    Console.WriteLine("User does not have the required role.");
                    return Unauthorized(new { message = "User does not have the required role." });
                }

                // Retrieve the 'BedrijfWagenparkbeheerders' associated with the logged-in user's email
                var bedrijfWagenparkbeheerder = await _context.BedrijfWagenparkbeheerders
                    .Include(bw => bw.Bedrijf)
                    .FirstOrDefaultAsync(bw => bw.Account.Email == email);

                if (bedrijfWagenparkbeheerder == null)
                {
                    Console.WriteLine("No associated Bedrijf found for the current user.");
                    return NotFound(new { message = "No associated Bedrijf found for the current user." });
                }

                var bedrijfId = bedrijfWagenparkbeheerder.bedrijf_id;

                // Log the retrieved bedrijfId
                Console.WriteLine($"Retrieved bedrijfId: {bedrijfId}");

                // Return the 'Bedrijf' Id
                return Ok(new { bedrijfId = bedrijfId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internal server error: {ex.Message}");
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
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
                return NotFound("Abonnement not found");
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
                return NotFound("Abonnement not found");
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
        // Ensure `abonnementId` is provided
        if (!data.TryGetProperty("abonnementId", out var abonnementIdProp))
        {
            return BadRequest(new { message = "Missing required field 'abonnementId'.", statusCode = 400 });
        }

        int abonnementId = abonnementIdProp.GetInt32();

        // Identify the current user and their bedrijf (assuming session or authentication provides user info)
        var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        if (emailClaim == null)
        {
            return Unauthorized(new { message = "User is not authenticated.", statusCode = 401 });
        }

        var email = emailClaim.Value;

        // Retrieve the 'BedrijfWagenparkbeheerders' associated with the logged-in user's email
        var bedrijfWagenparkbeheerder = await _context.BedrijfWagenparkbeheerders
            .Include(bw => bw.Bedrijf)
            .FirstOrDefaultAsync(bw => bw.Account.Email == email);

        if (bedrijfWagenparkbeheerder == null)
        {
            return NotFound(new { message = "Bedrijf not found for the current user.", statusCode = 404 });
        }

        var bedrijf = bedrijfWagenparkbeheerder.Bedrijf;

        // Find the specified abonnement
        var abonnement = await _context.Abonnement.FindAsync(abonnementId);
        if (abonnement == null)
        {
            return NotFound(new { message = "Abonnement not found.", statusCode = 404 });
        }

        // Update the bedrijf's abonnement
        bedrijf.AbonnementId = abonnement.Id;
        _context.Bedrijf.Update(bedrijf);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Abonnement {abonnement.Naam} successfully selected for Bedrijf {bedrijf.naam}", statusCode = 200 });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = $"Internal server error: {ex.Message}", statusCode = 500 });
    }
}
    }
}