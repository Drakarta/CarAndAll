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
using Org.BouncyCastle.Asn1.X509;


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

        [HttpGet("GetVerhuurAanvragen")]
        public async Task<IActionResult> GetVerhuurAanvragen()
        {
            try
            {
            var verhuurAanvragen = await _context.VerhuurAanvragen.Select(v => new 
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

        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus([FromBody] BackOfficeModel model)
        {
            try
            {
                var verhuurAanvraag = await _context.VerhuurAanvragen
                    .FirstOrDefaultAsync(v => v.AanvraagID == model.AanvraagID);
Console.WriteLine(model.AanvraagID);
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