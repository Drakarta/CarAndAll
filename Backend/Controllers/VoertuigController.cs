using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using Backend.Data;
using Backend.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoertuigController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public VoertuigController(ApplicationDbContext context)
        {
            _applicationDbContext = context;
        }

        [Authorize(Policy = "ParticuliereZakelijkeHuurder")]
        [HttpGet("Voertuigen")]
        public async Task<IActionResult> GetVoertuigen()
        {
            try
            {
            var voertuigIds = await _applicationDbContext.Voertuigen.Select(v => new 
                                    {
                                        voertuigID = v.VoertuigID,
                                        naam = v.Merk + " " + v.Type,
                                        categorie = v.Categorie,
                                        prijs_per_dag = v.Prijs_per_dag,
                                        verhuur_perioden = _applicationDbContext.VerhuurAanvragen
                                            .Where(va => va.VoertuigID == v.VoertuigID)
                                            .Select(va => new 
                                            {
                                                verhuur_start = va.Startdatum,
                                                verhuur_eind = va.Einddatum
                                            }).ToList()
                                    })
                                    .ToListAsync();

            return Ok (voertuigIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
