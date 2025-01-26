using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Entities;
using Microsoft.AspNetCore.Authorization;



namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FrontOfficeController : ControllerBase
    {
        protected readonly ApplicationDbContext _context;

        public FrontOfficeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "FrontOffice")]
        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus([FromBody] Request model)
        {
            var id = model.AanvraagID;
            var newStatus = model.NewStatus;
            var schade = model.SchadeInfo;

            var aanvraag = await _context.VerhuurAanvragen.FindAsync(id);
            if (aanvraag == null)
            {
                return NotFound("Aanvraag not found");
            }


            var validStatuses = new[] { "ingenomen", "uitgegeven", "in reparatie" };
            if (!validStatuses.Contains(newStatus.ToLower()))
            {
                return BadRequest("Invalid status");
            }

            var voertuigId = aanvraag.VoertuigID;
            if (!string.IsNullOrEmpty(schade))
            {
                var newSchade = new Schade { VoertuigID = voertuigId, schade = schade };
                _context.Schades.Add(newSchade);
            }

            aanvraag.Status = newStatus;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok(new { message = "Status updated successfully" });
        }

        [Authorize(Policy = "FrontOffice")]
        [HttpGet("GetVerhuurAanvragenWithStatus")]
        public async Task<IActionResult> GetVerhuurAanvragenWithStatus()
        {
            var requests = await _context.VerhuurAanvragen
                .Select(r => new { r.AanvraagID, r.Status })
                .Where(r => r.Status == "geaccepteerd" || r.Status == "uitgegeven")
                .ToListAsync();
            return Ok(requests);
        }
    }
    public class Request
    {
        public int AanvraagID { get; set; }
        public string NewStatus { get; set; } = string.Empty;
        public string? SchadeInfo { get; set; }
    }

}