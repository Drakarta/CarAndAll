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

namespace Backend.Controllers
{
    [ApiController]
    public abstract class BaseEmailController : ControllerBase
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IEmailSender _emailSender;

        protected BaseEmailController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        [HttpGet("emails")]
        public virtual async Task<IActionResult> GetEmails()
        {
            try
            {
                var accountEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var account_id = await _context.Account
                    .Where(a => a.Email == accountEmail.Value)
                    .Select(a => a.Id)
                    .FirstOrDefaultAsync();
                Console.WriteLine($"Logged-in user's AccountId: {account_id}");

                var BedrijfEigenaar_id = await _context.Bedrijf
                    .Where(b => b.Eigenaar == account_id)
                    .Select(b => b.Id)
                    .FirstOrDefaultAsync();
                Console.WriteLine($"Retrieved bedrijf_id: {BedrijfEigenaar_id}");

                if (BedrijfEigenaar_id == Guid.Empty)
                {
                    return Unauthorized("User is not associated with any company.");
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

        [HttpPost("addUserToCompany")]
        public virtual async Task<IActionResult> AddUserToCompany([FromBody] EmailModel model)
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
                var bedrijf_id = await _context.Bedrijf
                    .Where(b => b.Eigenaar == account_id)
                    .Select(b => b.Id)
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
                        wachtwoord = password
                    };
                    _context.Account.Add(newAccount);
                    await _context.SaveChangesAsync();
                    account = await _context.Account
                        .FirstOrDefaultAsync(a => string.Equals(a.Email, model.Email, StringComparison.OrdinalIgnoreCase));
                    // string context = $"{model.Email} {password} Dit zijn uw loggin gegevens";
                    // _emailSencer.SendEmail("pbt05@hotmail.nl", "Account gegevens", context);
                }

                var bedrijf = await _context.Bedrijf.FindAsync(bedrijf_id);

                var abbonement = await _context.Bedrijf
                    .Where(b => b.Id == bedrijf_id)
                    .Select(b => b.abonnement)
                    .FirstOrDefaultAsync();
                var CountAccountBedrijf = await _context.BedrijfAccounts
                    .Where(ab => ab.bedrijf_id == bedrijf_id)
                    .CountAsync();

                if (!EmailHelper.CheckAmountAllowedToAddToCompany(ToString(abbonement), CountAccountBedrijf))
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

        private string ToString(Abonnement? abbonement)
        {
            throw new NotImplementedException();
        }

        [HttpPost("removeUserFromCompany")]
        public virtual async Task<IActionResult> RemoveUserFromCompany([FromBody] EmailModel model)
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
                var bedrijf_id = await _context.Bedrijf
                    .Where(b => b.Eigenaar == account_id)
                    .Select(b => b.Id)
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
    }
}
