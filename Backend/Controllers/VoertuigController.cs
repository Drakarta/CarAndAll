using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using Backend.Data;
using Backend.Entities;

namespace Backend.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoertuigController : ControllerBase
    {
        private readonly ApplicationDbContext _voertuigDbContext;

        public VoertuigController(ApplicationDbContext context)
        {
            _voertuigDbContext = context;
        }

        [HttpGet("Voertuigen")]
        public async Task<IActionResult> GetVoertuigen()
        {
            try
            {
            var voertuigIds = await _voertuigDbContext.Voertuigen.Select(v => new{ naam = (v.Merk + ' ' + v.Type), categorie = v.Categorie}).ToListAsync();
            return Ok (voertuigIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
