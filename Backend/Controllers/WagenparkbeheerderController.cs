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
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WagenparkbeheerderController : ControllerBase
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IEmailSender _emailSender;

        public WagenparkbeheerderController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
            
        }

        [Authorize(Policy="Wagenparkbeheerder")]
        [HttpGet("emails")]
        public async Task<IActionResult> GetEmails()
        {
            try
            {
                var accountEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var account_id = await _context.Account
                    .Where(a => a.Email == accountEmail.Value)
                    .Select(a => a.Id)
                    .FirstOrDefaultAsync();

                var BedrijfEigenaar_id = await _context.BedrijfWagenparkbeheerders
                    .Where(bw => bw.account_id == account_id)
                    .Include(b => b.Bedrijf)
                    .Select(b => b.Bedrijf.Id)
                    .FirstOrDefaultAsync();

                if (BedrijfEigenaar_id == Guid.Empty)
                {
                    return Unauthorized("User is not associated with any company.");
                }
                var Abbonement = await _context.Bedrijf
                    .Where(b => b.Id == BedrijfEigenaar_id)
                    .Select(b => b.abonnement)
                    .FirstOrDefaultAsync();
                if (Abbonement == null)
                {
                    return NotFound("Company does not have any subscription.");
                }

                var accountEmails = await _context.Account
                    .Join(_context.BedrijfAccounts,
                          a => a.Id,
                          ab => ab.account_id,
                          (a, ab) => new { a.Email, ab.bedrijf_id })
                    .Where(result => result.bedrijf_id == BedrijfEigenaar_id)
                    .Select(result => result.Email)
                    .ToListAsync();
                return Ok(accountEmails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Policy="Wagenparkbeheerder")]
        [HttpPost("addUserToCompany")]
        public async Task<IActionResult> AddUserToCompany([FromBody] EmailModelAdd model)
        {
            try
            {
                if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    var errorDetails = new {
                        message = "FalseFormatEmail",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }
                var accountEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var reqeustEmailToLower = model.Email.ToLower();
                var lowerAccountEmail = accountEmail.Value.ToLower();
                if (lowerAccountEmail == reqeustEmailToLower)
                {
                    var errorDetails = new {
                        message = "User cannot add himself/herself to the company.",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                var account_id = await _context.Account
                    .Where(a => a.Email == accountEmail.Value)
                    .Select(a => a.Id)
                    .FirstOrDefaultAsync();;
                var bedrijf_id = await _context.BedrijfWagenparkbeheerders
                    .Where(bw => bw.account_id == account_id)
                    .Include(b => b.Bedrijf)
                    .Select(b => b.Bedrijf.Id)
                    .FirstOrDefaultAsync();

                if (bedrijf_id == Guid.Empty)
                {
                    var errorDetails = new {
                        message = "User is not associated with any company.",
                        statusCode = 401
                    };
                    return Unauthorized(errorDetails);
                }

                var domein = await _context.Bedrijf
                    .Where(b => b.Id == bedrijf_id)
                    .Select(b => b.Domein.ToLower())
                    .FirstOrDefaultAsync();
                if (domein == null || !EmailHelper.CheckDomeinAllowToAddToCompany(reqeustEmailToLower, domein))
                {
                    var errorDetails = new {
                        message = "FalseDomein",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                var account = await _context.Account
                    .FirstOrDefaultAsync(a => a.Email.ToLower() == reqeustEmailToLower);

                if (account == null)
                {
                    var password = Password.CreatePassword(8);
                    var newAccount = new Account
                    {
                        Id = Guid.NewGuid(),
                        Email = reqeustEmailToLower,
                        wachtwoord = password,
                        Rol = model.Role,
                    };
                    _context.Account.Add(newAccount);
                    await _context.SaveChangesAsync();
                    account = await _context.Account
                        .FirstOrDefaultAsync(a => string.Equals(a.Email, model.Email, StringComparison.OrdinalIgnoreCase));
                    // string context = $"{model.Email} {password} Dit zijn uw loggin gegevens";
                    // _emailSencer.SendEmail("pbt05@hotmail.nl", "Account gegevens", context);
                }

                var bedrijf = await _context.Bedrijf.FindAsync(bedrijf_id);

                var abonnementMaxNumbers = await _context.Bedrijf
                    .Where(b => b.Id == bedrijf_id)
                    .Select(b => b.abonnement.Max_medewerkers)
                    .FirstOrDefaultAsync();
                Console.WriteLine($"Max number of employees allowed: {abonnementMaxNumbers}");
                var CountAccountBedrijf = await _context.BedrijfAccounts
                    .Where(ab => ab.bedrijf_id == bedrijf_id)
                    .CountAsync();

                if (!EmailHelper.CheckAmountAllowedToAddToCompany(abonnementMaxNumbers, CountAccountBedrijf))
                {
                    var errorDetails = new {
                        message = "MaxNumber",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                var accountBedrijf = new BedrijfAccounts
                {
                    account_id = account.Id,
                    bedrijf_id = bedrijf_id,
                    Account = account,
                    Bedrijf = bedrijf ?? throw new InvalidOperationException("Bedrijf not found")
                };
                account.Rol = model.Role;
                if (model.Role == "Wagenparkbeheerder")
                {
                    var bedrijfWagenparkbeheerder = new BedrijfWagenparkbeheerders
                    {
                        account_id = account.Id,
                        bedrijf_id = bedrijf_id,
                        Account = account,
                        Bedrijf = bedrijf ?? throw new InvalidOperationException("Bedrijf not found")
                    };
                    _context.BedrijfWagenparkbeheerders.Add(bedrijfWagenparkbeheerder);
                }


                _context.BedrijfAccounts.Add(accountBedrijf);
                await _context.SaveChangesAsync();

                var succesDetails = new {
                    message = "User added to the company successfully.",
                    statusCode = 200
                };

                return Ok(succesDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Policy="Wagenparkbeheerder")]
        [HttpPost("removeUserFromCompany")]
        public async Task<IActionResult> RemoveUserFromCompany([FromBody] EmailModelRemove model)
        {
            try
            {
                if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    var errorDetails = new {
                        message = "This is not the correct format of an email.",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                var account = await _context.Account
                    .FirstOrDefaultAsync(a => a.Email.ToLower() == model.Email.ToLower());

                if (account == null)
                {
                    var errorDetails = new {
                        message = "Account with the provided email does not exist.",
                        statusCode = 404
                    };
                    return NotFound(errorDetails);
                }

                var accountEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var account_id = await _context.Account
                    .Where(a => a.Email == accountEmail.Value)
                    .Select(a => a.Id)
                    .FirstOrDefaultAsync();
                var bedrijf_id = await _context.BedrijfWagenparkbeheerders
                    .Where(bw => bw.account_id == account_id)
                    .Include(b => b.Bedrijf)
                    .Select(b => b.Bedrijf.Id)
                    .FirstOrDefaultAsync();

                var BedrijfNaam = await _context.Bedrijf
                    .Where(b => b.Id == bedrijf_id)
                    .Select(b => b.naam)
                    .FirstOrDefaultAsync();

                if (bedrijf_id == Guid.Empty)
                {
                    var errorDetails = new {
                        message = "User is not associated with any company.",
                        statusCode = 401
                    };
                    return Unauthorized(errorDetails);
                }

                var accountBedrijf = await _context.BedrijfAccounts
                    .FirstOrDefaultAsync(ab => ab.account_id == account.Id && ab.bedrijf_id == bedrijf_id);

                if (accountBedrijf == null)
                {
                    var errorDetails = new {
                        message = "User is not associated with company.",
                        statusCode = 404
                    };
                    return NotFound(errorDetails);
                }
                 var bedrijfEigenaar = await _context.Bedrijf
                    .Where(b => b.Id == bedrijf_id)
                    .Select(b => b.Eigenaar)
                    .FirstOrDefaultAsync();

                if (account.Id == bedrijfEigenaar)
                {
                    var errorDetails = new {
                        message = "Owner cannot be removed from the company.",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                  if (account.Rol == "Wagenparkbeheerder")
                {
                    var bedrijfWagenparkbeheerder = await _context.BedrijfWagenparkbeheerders
                        .FirstOrDefaultAsync(bw => bw.account_id == account.Id && bw.bedrijf_id == bedrijf_id);
                    
                    _context.BedrijfWagenparkbeheerders.Remove(bedrijfWagenparkbeheerder);
                }

                // string context = $"{model.Email} U bent verwijderd van het bedrijf:{BedrijfNaam}  ";
                //  _emailSencer.SendEmail("pbt05@hotmail.nl", "Account verwijderd", context);
               

                _context.BedrijfAccounts.Remove(accountBedrijf);
                await _context.SaveChangesAsync();
                var succesDetails = new {
                    message = "User removed from the company successfully.",
                    statusCode = 200
                };

                return Ok(succesDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Policy="Wagenparkbeheerder")]
        [HttpPost("GetVoertuigenPerUser")]
        public async Task<IActionResult> GetVoertuigenPerUser([FromBody] VoertuigUserModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email) || !Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    var errorDetails = new {
                        message = "Invalid email format.",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                var acc = await _context.Account
                    .FirstOrDefaultAsync(a => a.Email.ToLower() == model.Email.ToLower());

                if (acc == null)
                {
                    var errorDetails = new {
                        message = "Account with the provided email does not exist.",
                        statusCode = 404
                    };
                    return NotFound(errorDetails);
                }
                if ((model.maand != "Whole year") && (model.jaar == 0))
                {
                    int monthNumber = DateTime.ParseExact(model.maand, "MMMM", CultureInfo.InvariantCulture).Month;
                    var verhuurAanvragen = await _context.VerhuurAanvragen
                        .Where(v => v.Account.Id == acc.Id && (v.Startdatum.Month == monthNumber || v.Einddatum.Month == monthNumber))
                        .Include(v => v.Voertuig)
                        .Select(a => new VerhuurAanvraagDTO
                {
                    AanvraagID = a.AanvraagID,
                    Startdatum = a.Startdatum,
                    Einddatum = a.Einddatum,
                    Bestemming = a.Bestemming,
                    Kilometers = a.Kilometers,
                    Status = a.Status,
                    Voertuig = new VoertuigDTO
                    {
                        Merk = a.Voertuig.Merk,
                        Type = a.Voertuig.Type,
                        Kenteken = a.Voertuig.Kenteken,
                        Kleur = a.Voertuig.Kleur,
                        Prijs_per_dag = a.Voertuig.Prijs_per_dag
                    }
                }).ToListAsync();

                    return Ok(verhuurAanvragen);
                } 
                else if ((model.maand == "Whole year") && (model.jaar != 0))
                {
                    var verhuurAanvragen = await _context.VerhuurAanvragen
                        .Where(v => v.Account.Id == acc.Id && (v.Startdatum.Year == model.jaar || v.Einddatum.Year == model.jaar))
                        .Include(v => v.Voertuig)
                        .Select(a => new VerhuurAanvraagDTO
                {
                    AanvraagID = a.AanvraagID,
                    Startdatum = a.Startdatum,
                    Einddatum = a.Einddatum,
                    Bestemming = a.Bestemming,
                    Kilometers = a.Kilometers,
                    Status = a.Status,
                    Voertuig = new VoertuigDTO
                    {
                        Merk = a.Voertuig.Merk,
                        Type = a.Voertuig.Type,
                        Kenteken = a.Voertuig.Kenteken,
                        Kleur = a.Voertuig.Kleur,
                        Prijs_per_dag = a.Voertuig.Prijs_per_dag
                    }
                }).ToListAsync();

                    return Ok(verhuurAanvragen);
                }
                else if ((model.maand != "Whole year") && (model.jaar != 0))
                {
                    int monthNumber = DateTime.ParseExact(model.maand, "MMMM", CultureInfo.InvariantCulture).Month;
                    var verhuurAanvragen = await _context.VerhuurAanvragen
                        .Where(v => v.Account.Id == acc.Id && ((v.Startdatum.Year == model.jaar && v.Startdatum.Month == monthNumber) || (v.Einddatum.Year == model.jaar && v.Einddatum.Month == monthNumber)))
                        .Include(v => v.Voertuig)
                        .Select(a => new VerhuurAanvraagDTO
                {
                    AanvraagID = a.AanvraagID,
                    Startdatum = a.Startdatum,
                    Einddatum = a.Einddatum,
                    Bestemming = a.Bestemming,
                    Kilometers = a.Kilometers,
                    Status = a.Status,
                    Voertuig = new VoertuigDTO
                    {
                        Merk = a.Voertuig.Merk,
                        Type = a.Voertuig.Type,
                        Kenteken = a.Voertuig.Kenteken,
                        Kleur = a.Voertuig.Kleur,
                        Prijs_per_dag = a.Voertuig.Prijs_per_dag
                    }
                }).ToListAsync();

                    return Ok(verhuurAanvragen);
                }
                else
                {
                    var verhuurAanvragen = await _context.VerhuurAanvragen
                        .Where(v => v.Account.Id == acc.Id)
                        .Include(v => v.Voertuig)
                        .Select(a => new VerhuurAanvraagDTO
                {
                    AanvraagID = a.AanvraagID,
                    Startdatum = a.Startdatum,
                    Einddatum = a.Einddatum,
                    Bestemming = a.Bestemming,
                    Kilometers = a.Kilometers,
                    Status = a.Status,
                    Voertuig = new VoertuigDTO
                    {
                        Merk = a.Voertuig.Merk,
                        Type = a.Voertuig.Type,
                        Kenteken = a.Voertuig.Kenteken,
                        Kleur = a.Voertuig.Kleur,
                        Prijs_per_dag = a.Voertuig.Prijs_per_dag
                    }
                }).ToListAsync();

                    return Ok(verhuurAanvragen);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}
