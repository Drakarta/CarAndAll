using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Entities;
using Backend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AbonnementAanvraagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AbonnementAanvraagController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "Wagenparkbeheerder")]
        [HttpPost("create")]
        // Maak een nieuwe abonnement aanvraag aan in de database
        public async Task<IActionResult> CreateAbonnementAanvraag([FromBody] AbonnementAanvraagModel model)
        {
            try
            {
                Console.WriteLine("Ontvangen verzoek om AbonnementAanvraag aan te maken"); // Debugging statement
                Console.WriteLine($"Naam: {model.Naam}");

                if (string.IsNullOrEmpty(model.Naam))
                {
                    return BadRequest(new { message = "Het veld 'Naam' is verplicht.", statusCode = 400 });
                }

                var abonnement = await _context.Abonnement.FirstOrDefaultAsync(a => a.Naam == model.Naam);

                var accountEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (accountEmail == null)
                {
                    return NotFound("Gebruiker is niet geauthenticeerd.");
                }

                var account_id = await _context.Account
                    .Where(a => a.Email == accountEmail.Value)
                    .Select(a => a.Id)
                    .FirstOrDefaultAsync();

                var BedrijfId = await _context.BedrijfWagenparkbeheerders
                    .Where(bw => bw.account_id == account_id)
                    .Include(b => b.Bedrijf)
                    .Select(bw => bw.bedrijf_id)
                    .FirstOrDefaultAsync();

                if (BedrijfId == Guid.Empty)
                {
                    return BadRequest(new { message = "Het veld 'BedrijfId' is verplicht.", statusCode = 400 });
                }

                var bedrijf = await _context.Bedrijf.Include(b => b.abonnement).FirstOrDefaultAsync(b => b.Id == BedrijfId);
                if (bedrijf == null)
                {
                    return NotFound(new { message = "Bedrijf niet gevonden.", statusCode = 404 });
                }

                var abonnementAanvraag = new AbonnementAanvraag
                {
                    Naam = model.Naam,
                    Beschrijving = abonnement?.Beschrijving ?? "Geen beschrijving beschikbaar",
                    PrijsMultiplier = abonnement.Prijs_multiplier,
                    MaxMedewerkers = abonnement.Max_medewerkers,
                    BedrijfId = BedrijfId,
                    Bedrijf = bedrijf,
                    Status = "In behandeling"
                };

                await _context.AbonnementAanvragen.AddAsync(abonnementAanvraag);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Abonnement aanvraag succesvol ingediend.", statusCode = 200 });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Interne serverfout: {ex.Message}"); // Debugging statement
                return StatusCode(500, new { message = $"Interne serverfout: {ex.Message}", statusCode = 500 });
            }
        }

        [HttpGet("GetAbonnementAanvragen")]
        // Haal alle abonnement aanvragen op die in behandeling zijn
        public async Task<ActionResult<IEnumerable<AbonnementAanvraag>>> GetAbonnementAanvragen()
        {
            try
            {
                var aanvragen = await _context.AbonnementAanvragen.Where(a => a.Status == "In behandeling").ToListAsync();
                if (aanvragen.Count == 0)
                {
                    return NotFound(new { message = "Geen abonnement aanvragen gevonden", statusCode = 404 });
                }
                return Ok(aanvragen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Interne serverfout: {ex.Message}", statusCode = 500 });
            }
        }

        [Authorize(Policy = "BackOffice")]
        [HttpPost("ChangeStatus")]
        // Verander de status van een abonnement aanvraag
        public async Task<IActionResult> ChangeStatus([FromBody] ChangeStatusModel model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Status))
                {
                    return BadRequest(new { message = "Ongeldige invoer", statusCode = 400 });
                }

                var aanvraag = await _context.AbonnementAanvragen.FindAsync(model.AanvraagID);
                if (aanvraag == null)
                {
                    return NotFound(new { message = "Abonnement aanvraag niet gevonden", statusCode = 404 });
                }
                
                var abonnement = await _context.Abonnement.FirstOrDefaultAsync(a => a.Naam == aanvraag.Naam);
                if (abonnement == null)
                {
                    return NotFound(new { message = "Abonnement niet gevonden", statusCode = 404 });
                }
                if (model.Status == "Geaccepteerd")
                {
                    var bedrijf = await _context.Bedrijf.FindAsync(aanvraag.BedrijfId);
                    if (bedrijf == null)
                    {
                        return NotFound(new { message = "Bedrijf niet gevonden", statusCode = 404 });
                    }
                    bedrijf.AbonnementId = abonnement.Id;
                    _context.Bedrijf.Update(bedrijf);
                }

                aanvraag.Status = model.Status;

                _context.AbonnementAanvragen.Update(aanvraag);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Status succesvol bijgewerkt", statusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Interne serverfout: {ex.Message}", statusCode = 500 });
            }
        }
    }
}