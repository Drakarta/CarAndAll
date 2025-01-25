using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;


namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BackOfficeController : ControllerBase
    {
          protected readonly ApplicationDbContext _context;

        public BackOfficeController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Verhuuraanvragen ophalen die 'In behandeling' zijn zodat de backoffice medewerker deze kan verwerken
        [Authorize(Policy = "Backofficemedewerker")]
        [HttpGet("GetVerhuurAanvragen")]
        public async Task<IActionResult> GetVerhuurAanvragen()
        {
            try
            {
            var verhuurAanvragen = await _context.VerhuurAanvragen.Where(v => v.Status == "In behandeling").Select(v => new 
                                    {
                                        aanvraagID = v.AanvraagID,
                                        startdatum = v.Startdatum,
                                        einddatum = v.Einddatum,
                                        bestemming = v.Bestemming,
                                        kilometers = v.Kilometers,
                                        status = v.Status,
                                        voertuig = _context.Voertuigen
                                            .Where(vt => vt.VoertuigID == v.VoertuigID)
                                            .Select(va => new 
                                            {
                                                voertuig_naam = va.Merk + " " + va.Type,
                                                voertuig_status = va.Status
                                            }).ToList()
                                    })
                                    .ToListAsync();

            return Ok (verhuurAanvragen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //Verwerken van de verhuuraanvragen aan de hand van de status zetten naar 'Geaccepteerd' of 'Afgewezen'
        [Authorize(Policy = "Backofficemedewerker")]
        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus([FromBody] BackOfficeModel model)
        {
            try
            {
                var verhuurAanvraag = await _context.VerhuurAanvragen
                    .FirstOrDefaultAsync(v => v.AanvraagID == model.AanvraagID);

                if (verhuurAanvraag == null)
                {
                    var errorDetails = new {
                        message = "The 'Verhuuraanvraag' is empty or 0",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }
                if(verhuurAanvraag != null){
                     verhuurAanvraag.Status = model.Status;
                     await _context.SaveChangesAsync();
                }

                var succes = new {
                    message = "Verhuur aanvraag status succesvol aangepast.",
                    statusCode = 200
                };
                return Ok (succes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}