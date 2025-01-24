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
        [HttpGet("currentBedrijf")]
public async Task<IActionResult> GetCurrentBedrijf()
{
    try
    {
        // Retrieve the current user based on the logged-in session or claims
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user's identifier (userId)

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        // Convert the userId to Guid if necessary (assuming the userId is stored as Guid)
        if (!Guid.TryParse(userId, out Guid parsedUserId))
        {
            return BadRequest(new { message = "Invalid user ID format." });
        }

        // Retrieve the 'Bedrijf' associated with the logged-in user
        var bedrijf = await _context.Bedrijf
            .Where(b => b.BedrijfAccounts.Any(ba => ba.account_id == parsedUserId))  // Use 'account_id' instead of 'AccountId'
            .Select(b => new { b.Id, b.naam })
            .FirstOrDefaultAsync();

        if (bedrijf == null)
        {
            return NotFound(new { message = "No associated Bedrijf found for the current user." });
        }

        // Return the 'Bedrijf' Id
        return Ok(new { bedrijfId = bedrijf.Id });
    }
    catch (Exception ex)
    {
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
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Use 'NameIdentifier' to get the user ID
        if (userId == null)
        {
            return Unauthorized(new { message = "User is not authenticated.", statusCode = 401 });
        }

        // Convert to Guid if necessary
        if (!Guid.TryParse(userId, out Guid parsedUserId))
        {
            return BadRequest(new { message = "Invalid user ID format.", statusCode = 400 });
        }

        // Retrieve the 'Bedrijf' associated with the current user
        var bedrijf = await _context.Bedrijf.FirstOrDefaultAsync(b => b.Eigenaar.ToString() == userId);
        if (bedrijf == null)
        {
            return NotFound(new { message = "Bedrijf not found for the current user.", statusCode = 404 });
        }

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
