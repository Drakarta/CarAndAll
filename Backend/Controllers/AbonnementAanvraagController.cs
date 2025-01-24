using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Entities;
using Backend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> CreateAbonnementAanvraag([FromBody] AbonnementAanvraag model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Naam))
                {
                    return BadRequest(new { message = "The 'Naam' field is required.", statusCode = 400 });
                }

                if (model.PrijsMultiplier <= 0)
                {
                    return BadRequest(new { message = "The 'PrijsMultiplier' must be greater than zero.", statusCode = 400 });
                }

                if (model.BedrijfId == Guid.Empty)
                {
                    return BadRequest(new { message = "The 'BedrijfId' field is required.", statusCode = 400 });
                }

                var bedrijf = await _context.Bedrijf.Include(b => b.abonnement).FirstOrDefaultAsync(b => b.Id == model.BedrijfId);
                if (bedrijf == null)
                {
                    return NotFound(new { message = "Bedrijf not found.", statusCode = 404 });
                }

                var abonnementAanvraag = new AbonnementAanvraag
                {
                    Naam = model.Naam,
                    Beschrijving = model.Beschrijving,
                    PrijsMultiplier = model.PrijsMultiplier,
                    MaxMedewerkers = model.MaxMedewerkers,
                    BedrijfId = model.BedrijfId,
                    Bedrijf = bedrijf,
                    Status = "In behandeling"
                };

                await _context.AbonnementAanvragen.AddAsync(abonnementAanvraag);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Abonnement aanvraag succesvol ingediend.", statusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}", statusCode = 500 });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AbonnementAanvraag>>> GetAbonnementAanvragen()
        {
            try
            {
                var aanvragen = await _context.AbonnementAanvragen.ToListAsync();
                return Ok(aanvragen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}", statusCode = 500 });
            }
        }

        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus([FromBody] ChangeStatusModel model)
        {
            try
            {
                if (model == null || model.AanvraagID == Guid.Empty || string.IsNullOrEmpty(model.Status))
                {
                    return BadRequest(new { message = "Invalid input", statusCode = 400 });
                }

                var aanvraag = await _context.AbonnementAanvragen.FindAsync(model.AanvraagID);
                if (aanvraag == null)
                {
                    return NotFound(new { message = "Abonnement aanvraag not found", statusCode = 404 });
                }

                aanvraag.Status = model.Status;

                _context.AbonnementAanvragen.Update(aanvraag);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Status successfully updated", statusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}", statusCode = 500 });
            }
        }
    }
}
