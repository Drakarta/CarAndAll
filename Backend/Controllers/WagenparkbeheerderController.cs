using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Backend.Data;
using Backend.Entities;
using Backend.Interfaces;
using Backend.Helpers;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Globalization;
using BC = BCrypt.Net.BCrypt;
using Backend.Services;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WagenparkbeheerderController : ControllerBase
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IEmailSender _emailSender;
        protected readonly VerhuurAanvraagService _VerhuurAanvraagService;

        public WagenparkbeheerderController(ApplicationDbContext context, IEmailSender emailSender, VerhuurAanvraagService verhuurAanvraagService)
        {
            _context = context;
            _emailSender = emailSender;
            _VerhuurAanvraagService = verhuurAanvraagService;

        }

        [Authorize(Policy = "Wagenparkbeheerder")]
        [HttpGet("emails")]
        public async Task<IActionResult> GetEmails()
        {
            try
            {
                var accountEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (accountEmail == null)
                {
                    return NotFound("User is not authenticated.");
                }

                var account_id = await _context.Account
                    .Where(a => a.Email == accountEmail.Value)
                    .Select(a => a.Id)
                    .FirstOrDefaultAsync();


                var BedrijfEigenaar_id = await _context.BedrijfWagenparkbeheerders
                    .Where(bw => bw.account_id == account_id)
                    .Include(b => b.Bedrijf)
                    .Select(bw => bw.bedrijf_id)
                    .FirstOrDefaultAsync();


                if (BedrijfEigenaar_id == Guid.Empty)
                {
                    return Unauthorized("User is not associated with any company.");
                }
                var Abonement = await _context.Bedrijf
                    .Where(b => b.Id == BedrijfEigenaar_id)
                    .Select(b => b.AbonnementId)
                    .FirstOrDefaultAsync();
                if (Abonement == null)
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
        [Authorize(Policy = "Wagenparkbeheerder")]
        [HttpPost("addAccountToCompany")]
        public async Task<IActionResult> AddAccountToCompany([FromBody] EmailModelAdd model)
        {
            try
            {
                if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    var errorDetails = new
                    {
                        message = "FalseFormatEmail",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }
                var accountEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (accountEmail == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                var reqeustEmailToLower = model.Email.ToLower();
                var lowerAccountEmail = accountEmail.Value.ToLower();
                if (lowerAccountEmail == reqeustEmailToLower)
                {
                    var errorDetails = new
                    {
                        message = "User cannot add himself/herself to the company.",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                var account_id = await _context.Account
                    .Where(a => a.Email == accountEmail.Value)
                    .Select(a => a.Id)
                    .FirstOrDefaultAsync();
                var bedrijf_id = await _context.BedrijfWagenparkbeheerders
                    .Where(bw => bw.account_id == account_id)
                    .Select(bw => bw.bedrijf_id)
                    .FirstOrDefaultAsync();

                if (bedrijf_id == Guid.Empty)
                {
                    var errorDetails = new
                    {
                        message = "User is not associated with any company.",
                        statusCode = 401
                    };
                    return Unauthorized(errorDetails);
                }

                var domein = await _context.Bedrijf
                    .Where(b => b.Id == bedrijf_id)
                    .Select(b => b.Domein != null ? b.Domein.ToLower() : string.Empty)
                    .FirstOrDefaultAsync();
                if (domein == null || !EmailHelper.CheckDomeinAllowToAddToCompany(reqeustEmailToLower, domein))
                {
                    var errorDetails = new
                    {
                        message = "FalseDomein",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                var account = await _context.Account
                    .FirstOrDefaultAsync(a => a.Email.ToLower() == reqeustEmailToLower);

                if (account == null)
                {
                    var password = Password.CreatePassword(12);
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
                        .FirstOrDefaultAsync(a => string.Equals(a.Email.ToLower(), model.Email.ToLower()));
                    // string context = $"{model.Email} {password} Dit zijn uw loggin gegevens";
                    // _emailSencer.SendEmail("pbt05@hotmail.nl", "Account gegevens", context);

                }



                var bedrijf = await _context.Bedrijf.FindAsync(bedrijf_id);


                if (bedrijf == null)
                {
                    return NotFound("Company not found.");
                }

                var abonnementMaxNumbers = await _context.Bedrijf
                    .Where(b => b.Id == bedrijf.Id)
                    .Select(b => b.abonnement != null ? b.abonnement.Max_medewerkers : 0)
                    .FirstOrDefaultAsync();


                var CountAccountBedrijf = await _context.BedrijfAccounts
                    .Where(ab => ab.bedrijf_id == bedrijf_id)
                    .CountAsync();



                if (!EmailHelper.CheckAmountAllowedToAddToCompany(abonnementMaxNumbers, CountAccountBedrijf))
                {
                    var errorDetails = new
                    {
                        message = "MaxNumber",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                var accountBedrijf = new BedrijfAccounts
                {
                    account_id = account?.Id ?? throw new InvalidOperationException("Account not found"),
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

                var succesDetails = new
                {
                    message = "User added to the company successfully.",
                    statusCode = 200
                };

                return Ok(succesDetails);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Policy = "Wagenparkbeheerder")]
        [HttpPost("removeUserFromCompany")]
        public async Task<IActionResult> RemoveUserFromCompany([FromBody] EmailModelRemove model)
        {
            try
            {
                if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    var errorDetails = new
                    {
                        message = "This is not the correct format of an email.",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                var account = await _context.Account
                    .FirstOrDefaultAsync(a => a.Email.Equals(model.Email, StringComparison.CurrentCultureIgnoreCase));

                if (account == null)
                {
                    var errorDetails = new
                    {
                        message = "Account with the provided email does not exist.",
                        statusCode = 404
                    };
                    return NotFound(errorDetails);
                }

                var accountEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (accountEmail == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
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
                    var errorDetails = new
                    {
                        message = "User is not associated with any company.",
                        statusCode = 401
                    };
                    return Unauthorized(errorDetails);
                }

                var accountBedrijf = await _context.BedrijfAccounts
                    .FirstOrDefaultAsync(ab => ab.account_id == account.Id && ab.bedrijf_id == bedrijf_id);

                if (accountBedrijf == null)
                {
                    var errorDetails = new
                    {
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
                    var errorDetails = new
                    {
                        message = "Owner cannot be removed from the company.",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                if (account.Rol == "Wagenparkbeheerder")
                {
                    var bedrijfWagenparkbeheerder = await _context.BedrijfWagenparkbeheerders
                        .FirstOrDefaultAsync(bw => bw.account_id == account.Id && bw.bedrijf_id == bedrijf_id);
                        if (bedrijfWagenparkbeheerder != null)
                        {
                            _context.BedrijfWagenparkbeheerders.Remove(bedrijfWagenparkbeheerder);
                        } else {
                            return NotFound("User is not a fleet manager.");
                        }

                   
                }

                // string context = $"{model.Email} U bent verwijderd van het bedrijf:{BedrijfNaam}  ";
                //  _emailSencer.SendEmail("pbt05@hotmail.nl", "Account verwijderd", context);


                _context.BedrijfAccounts.Remove(accountBedrijf);
                await _context.SaveChangesAsync();
                var succesDetails = new
                {
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
        [Authorize(Policy = "Wagenparkbeheerder")]
        [HttpPost("GetVoertuigenPerUser")]
        public async Task<IActionResult> GetVoertuigenPerUser([FromBody] VoertuigUserModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email) || !Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    return BadRequest(new { message = "Invalid email format.", statusCode = 400 });
                }

                var acc = await _context.Account.FirstOrDefaultAsync(a => a.Email.ToLower() == model.Email.ToLower());
                if (acc == null)
                {
                    return NotFound(new { message = "Account with the provided email does not exist.", statusCode = 404 });
                }

                var verhuurAanvragen = await _VerhuurAanvraagService.GetVerhuurAanvragen(model, acc.Id);
                
                if (verhuurAanvragen == null)
                {
                    return StatusCode(204);
                }
                return Ok(verhuurAanvragen);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
