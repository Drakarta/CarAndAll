using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

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

        //Voertuigen ophalen die niet verwijderd zijn
        [Authorize(Policy = "ParticuliereZakelijkeHuurderBackOffice")]
        [HttpGet("Voertuigen")]
        public async Task<IActionResult> GetVoertuigen()
        {
            try
            {
            var abonnementMultiplier = 1.0;
            var accountRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var accountEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var account_id = await _applicationDbContext.Account
                    .Where(a => a.Email == accountEmail.Value)
                    .Select(a => a.Id)
                    .FirstOrDefaultAsync();
            
            if(!accountRole.Value.IsNullOrEmpty() && accountRole.Value == "Zakelijkeklant"){
            var bedrijf_id = await _applicationDbContext.BedrijfAccounts
                    .Where(a => a.account_id == account_id)
                    .Select(a => a.bedrijf_id)
                    .FirstOrDefaultAsync();
            
            var bedrijfAbonnement = await _applicationDbContext.Bedrijf
                    .Where(b => b.Id == bedrijf_id)
                    .Select(b => b.AbonnementId)
                    .FirstOrDefaultAsync();

            abonnementMultiplier = await _applicationDbContext.Abonnement
                    .Where(a => a.Id == bedrijfAbonnement)
                    .Select(a => a.Prijs_multiplier)
                    .FirstOrDefaultAsync();
            }

            if(abonnementMultiplier == 0 || abonnementMultiplier == 0.0){
                abonnementMultiplier = 1;
            }   
            var voertuigIds = await _applicationDbContext.Voertuigen.Where(v => v.Deleted_on == null)
                                    .Select(v => new 
                                    {
                                        voertuigID = v.VoertuigID,
                                        naam = v.Merk + " " + v.Type,
                                        voertuig_categorie = v.voertuig_categorie,
                                        prijs_per_dag = v.Prijs_per_dag * abonnementMultiplier,
                                        status = v.Status,
                                        verhuur_perioden = _applicationDbContext.VerhuurAanvragen
                                            .Where((va => va.VoertuigID == v.VoertuigID))
                                            .Select(va => new 
                                            {
                                                verhuur_start = va.Startdatum,
                                                verhuur_eind = va.Einddatum
                                            }).ToList()
                                    })
                                    .ToListAsync();
            
                return Ok(voertuigIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        //Voertuigen aanmaken aan de hand van de ingevulde data die in de model gezet zijn
        [Authorize(Policy = "Backofficemedewerker")]
        [HttpPost("createVoertuig")]
        public async Task<IActionResult> CreateVoertuig([FromBody] CreateVoertuigModel model)
        {
            try
            {
                if (model.voertuig_categorie == "Null" || model.voertuig_categorie == null || model.Merk.IsNullOrEmpty() || model.Type.IsNullOrEmpty() || model.Kenteken.IsNullOrEmpty() || model.Kleur.IsNullOrEmpty() || model.Aanschafjaar.IsNullOrEmpty() || model.Prijs_per_dag == 0 || model.Prijs_per_dag < 1)
                {
                    var errorDetails = new {
                        message = "One or more of the required (*) fields are empty or 0.",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }
                if(model.voertuig_categorie == "Auto"){

                if (model.Aantal_deuren == null || model.Aantal_deuren == 0)
                {
                    var errorDetails = new {
                        message = "The 'Aantal deuren' input is empty or 0",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                if (model.Elektrisch == null)
                {
                    var errorDetails = new {
                        message = "The 'Aantal deuren' input is empty or 0",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                    var auto1 = new Auto
                    {
                        Merk = model.Merk,
                        Type = model.Type,
                        Kenteken = model.Kenteken,
                        Kleur = model.Kleur,
                        Aanschafjaar = model.Aanschafjaar,
                        Prijs_per_dag = model.Prijs_per_dag,
                        Status = model.Status,
                        Aantal_deuren = (int)model.Aantal_deuren,
                        Elektrisch = (bool)model.Elektrisch
                    };
                    _applicationDbContext.Voertuigen.Add(auto1);
                }else if(model.voertuig_categorie == "Camper"){

                if (model.Aantal_slaapplekken == null || model.Aantal_slaapplekken == 0)
                {
                    var errorDetails = new {
                        message = "The 'Aantal slaapplekken' input is empty or 0",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                if (model.Elektrisch == null)
                {
                    var errorDetails = new {
                        message = "The 'Aantal deuren' input is empty or 0",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }
                    var camper1 = new Camper
                    {
                        Merk = model.Merk,
                        Type = model.Type,
                        Kenteken = model.Kenteken,
                        Kleur = model.Kleur,
                        Aanschafjaar = model.Aanschafjaar,
                        Prijs_per_dag = model.Prijs_per_dag,
                        Status = model.Status,
                        Aantal_slaapplekken = (int)model.Aantal_slaapplekken,
                        Elektrisch = (bool)model.Elektrisch
                    };
                    _applicationDbContext.Voertuigen.Add(camper1);
                }else if(model.voertuig_categorie == "Caravan"){

                if (model.Aantal_slaapplekken == null || model.Aantal_slaapplekken == 0)
                {
                    var errorDetails = new {
                        message = "The 'Aantal slaapplekken' input is empty or 0",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }
                if (model.Gewicht_kg == null || model.Gewicht_kg == 0)
                {
                    var errorDetails = new {
                        message = "The 'Gewicht KG' input is empty or 0",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                    var caravan1 = new Caravan
                    {
                         Merk = model.Merk,
                        Type = model.Type,
                        Kenteken = model.Kenteken,
                        Kleur = model.Kleur,
                        Aanschafjaar = model.Aanschafjaar,
                        Prijs_per_dag = model.Prijs_per_dag,
                        Status = model.Status,
                        Aantal_slaapplekken = (int)model.Aantal_slaapplekken,
                        Gewicht_kg = (int)model.Gewicht_kg
                    };
                    _applicationDbContext.Voertuigen.Add(caravan1);
                }

                await _applicationDbContext.SaveChangesAsync();

                var succes = new {
                    message = "Voertuig succesvol aangemaakt.",
                    statusCode = 200
                };
                return Ok (succes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //Voertuig ophalen aan de hand van het VoertuigID
        [Authorize(Policy = "Backofficemedewerker")]
        [HttpGet("getVoertuigByID")]
        public async Task<IActionResult> GetVoertuigByID([FromQuery] int voertuigID)
        {
            try
            {
            var voertuigCategorie = await _applicationDbContext.Voertuigen
                    .FirstOrDefaultAsync(v => v.VoertuigID == voertuigID);

            
            if (voertuigCategorie == null)
            {
                return NotFound("Voertuig not found.");
            }

            if(voertuigCategorie.voertuig_categorie == "Auto"){
                var auto = await _applicationDbContext.Voertuigen
                    .OfType<Auto>()
                    .FirstOrDefaultAsync(v => v.VoertuigID == voertuigID);
                 return Ok (new Auto{
                    VoertuigID = auto.VoertuigID,
                    Merk = auto.Merk,
                    Type = auto.Type,
                    Kenteken = auto.Kenteken,
                    Kleur = auto.Kleur,
                    Aanschafjaar = auto.Aanschafjaar,
                    Prijs_per_dag = auto.Prijs_per_dag,
                    Status = auto.Status,
                    Aantal_deuren = auto.Aantal_deuren,
                    Elektrisch = auto.Elektrisch
                });
            }else if(voertuigCategorie.voertuig_categorie == "Camper"){
                var camper = await _applicationDbContext.Voertuigen
                    .OfType<Camper>()
                    .FirstOrDefaultAsync(v => v.VoertuigID == voertuigID);
                 return Ok (new Camper{
                    VoertuigID = camper.VoertuigID,
                    Merk = camper.Merk,
                    Type = camper.Type,
                    Kenteken = camper.Kenteken,
                    Kleur = camper.Kleur,
                    Aanschafjaar = camper.Aanschafjaar,
                    Prijs_per_dag = camper.Prijs_per_dag,
                    Status = camper.Status,
                    Aantal_slaapplekken = camper.Aantal_slaapplekken,
                    Elektrisch = camper.Elektrisch
                });
            }else if(voertuigCategorie.voertuig_categorie == "Caravan"){
                var caravan = await _applicationDbContext.Voertuigen
                    .OfType<Caravan>()
                    .FirstOrDefaultAsync(v => v.VoertuigID == voertuigID);
                 return Ok (new Caravan{
                    VoertuigID = caravan.VoertuigID,
                    Merk = caravan.Merk,
                    Type = caravan.Type,
                    Kenteken = caravan.Kenteken,
                    Kleur = caravan.Kleur,
                    Aanschafjaar = caravan.Aanschafjaar,
                    Prijs_per_dag = caravan.Prijs_per_dag,
                    Status = caravan.Status,
                    Aantal_slaapplekken = caravan.Aantal_slaapplekken,
                    Gewicht_kg = caravan.Gewicht_kg
                });
            }

                return NotFound("Voertuig / voertuig categorie not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //Voertuigen updaten aan de hand van het update voertuig model
        [Authorize(Policy = "Backofficemedewerker")]
        [HttpPost("updateVoertuig")]
        public async Task<IActionResult> UpdateVoertuig([FromBody] UpdateVoertuigModel model)
        {
            try
            {
                if (model.voertuig_categorie == "Null" || model.voertuig_categorie == null || model.Merk.IsNullOrEmpty() || model.Type.IsNullOrEmpty() || model.Kenteken.IsNullOrEmpty() || model.Kleur.IsNullOrEmpty() || model.Aanschafjaar.IsNullOrEmpty() || model.Prijs_per_dag == 0 || model.Prijs_per_dag < 1)
                {
                    var errorDetails = new {
                        message = "One or more of the required (*) fields are empty or 0.",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }
                if(model.voertuig_categorie == "Auto"){

                if (model.Aantal_deuren == null || model.Aantal_deuren == 0)
                {
                    var errorDetails = new {
                        message = "The 'Aantal deuren' input is empty or 0",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                if (model.Elektrisch == null)
                {
                    var errorDetails = new {
                        message = "The 'Aantal deuren' input is empty or 0",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                 var auto = await _applicationDbContext.Voertuigen
                    .OfType<Auto>()
                    .FirstOrDefaultAsync(v => v.VoertuigID == model.VoertuigID);

                    if(auto != null){
                        auto.Merk = model.Merk;
                        auto.Type = model.Type;
                        auto.Kenteken = model.Kenteken;
                        auto.Kleur = model.Kleur;
                        auto.Aanschafjaar = model.Aanschafjaar;
                        auto.Prijs_per_dag = model.Prijs_per_dag;
                        auto.Status = model.Status;
                        auto.Aantal_deuren = (int)model.Aantal_deuren;
                        auto.Elektrisch = (bool)model.Elektrisch;

                        await _applicationDbContext.SaveChangesAsync();
                    };
                }else if(model.voertuig_categorie == "Camper"){

                if (model.Aantal_slaapplekken == null || model.Aantal_slaapplekken == 0)
                {
                    var errorDetails = new {
                        message = "The 'Aantal slaapplekken' input is empty or 0",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                if (model.Elektrisch == null)
                {
                    var errorDetails = new {
                        message = "The 'Aantal deuren' input is empty or 0",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }
                    var camper = await _applicationDbContext.Voertuigen
                    .OfType<Camper>()
                    .FirstOrDefaultAsync(v => v.VoertuigID == model.VoertuigID);

                    if(camper != null){
                        camper.Merk = model.Merk;
                        camper.Type = model.Type;
                        camper.Kenteken = model.Kenteken;
                        camper.Kleur = model.Kleur;
                        camper.Aanschafjaar = model.Aanschafjaar;
                        camper.Prijs_per_dag = model.Prijs_per_dag;
                        camper.Status = model.Status;
                        camper.Aantal_slaapplekken = (int)model.Aantal_slaapplekken;
                        camper.Elektrisch = (bool)model.Elektrisch;

                        await _applicationDbContext.SaveChangesAsync();
                    };
                }else if(model.voertuig_categorie == "Caravan"){

                if (model.Aantal_slaapplekken == null || model.Aantal_slaapplekken == 0)
                {
                    var errorDetails = new {
                        message = "The 'Aantal slaapplekken' input is empty or 0",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }
                if (model.Gewicht_kg == null || model.Gewicht_kg == 0)
                {
                    var errorDetails = new {
                        message = "The 'Gewicht KG' input is empty or 0",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                    var caravan = await _applicationDbContext.Voertuigen
                    .OfType<Caravan>()
                    .FirstOrDefaultAsync(v => v.VoertuigID == model.VoertuigID);

                    if(caravan != null){
                        caravan.Merk = model.Merk;
                        caravan.Type = model.Type;
                        caravan.Kenteken = model.Kenteken;
                        caravan.Kleur = model.Kleur;
                        caravan.Aanschafjaar = model.Aanschafjaar;
                        caravan.Prijs_per_dag = model.Prijs_per_dag;
                        caravan.Status = model.Status;
                        caravan.Aantal_slaapplekken = (int)model.Aantal_slaapplekken;
                        caravan.Gewicht_kg = (int)model.Gewicht_kg;

                        await _applicationDbContext.SaveChangesAsync();
                    };
                }

                var succes = new {
                    message = "Voertuig succesvol geupdate.",
                    statusCode = 200
                };
                return Ok (succes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //Voertuig volledig verwijderen als hij geen verhuuraanvragen heeft en alleen 'soft' deleten als hij wel verhuuraanvragen heeft
        [Authorize(Policy = "Backofficemedewerker")]
        [HttpDelete("deleteVoertuig")]
        public async Task<IActionResult> DeleteVoertuig([FromBody] DeleteVoertuigModel model)
        {
            try
            {
                var voertuig = await _applicationDbContext.Voertuigen
                .FirstOrDefaultAsync(v => v.VoertuigID == model.VoertuigID);

                if (voertuig == null)
                {
                    return NotFound(new { message = "Voertuig not found.", statusCode = 404 });
                }

                var verhuuraanvragenCount = await _applicationDbContext.VerhuurAanvragen
                    .Where(vr => vr.VoertuigID == model.VoertuigID)
                    .CountAsync();
                
                if(verhuuraanvragenCount == 0){
                    _applicationDbContext.Voertuigen.Remove(voertuig);
                }else {
                    voertuig.Deleted_on = DateTime.Today;
                }

                await _applicationDbContext.SaveChangesAsync();

                var succes = new {
                    message = "Voertuig succesvol geupdate.",
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
