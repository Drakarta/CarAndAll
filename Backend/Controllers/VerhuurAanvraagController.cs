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

       [HttpPost("ChangeStatus")]
public virtual async Task<IActionResult> ChangeStatus([FromBody] Request model) {
    var id = model.AanvraagID;
    var newStatus = model.NewStatus;
    var schade = model.SchadeInfo;

    var aanvraag = await _context.VerhuurAanvragen.FindAsync(id);
    if (aanvraag == null) {
        return NotFound("Aanvraag not found");
    }


    var validStatuses = new[] { "ingenomen", "uitgegeven", "in reparatie" }; 
    if (!validStatuses.Contains(newStatus.ToLower())) {
        return BadRequest("Invalid status");
    }

    var voertuigId = aanvraag.VoertuigID;
    if (!string.IsNullOrEmpty(schade)) {
        var newSchade = new Schade { VoertuigID = voertuigId, schade = schade };
        _context.Schades.Add(newSchade);
    }

    aanvraag.Status = newStatus;
    
    try {
        await _context.SaveChangesAsync();
    } catch (Exception ex) {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }

    return Ok(new { message = "Status updated successfully" });
}

        [HttpGet("GetVerhuurAanvragenWithStatus")]
        public virtual async Task<IActionResult> GetVerhuurAanvragenWithStatus()
        {
            var requests = await _context.VerhuurAanvragen
                .Select(r => new { r.AanvraagID, r.Status})
                .Where(r => r.Status == "geaccepteerd")
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