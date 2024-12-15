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

namespace Backend.Controllers
{
    [ApiController]
    public abstract class BaseEmailController : ControllerBase
    {
        protected readonly ApplicationDbContext _emailDbContext;
        protected readonly IUserService _userService;
        protected readonly IEmailSender _emailSender;

        protected BaseEmailController(ApplicationDbContext context, IUserService userService, IEmailSender emailSender)
        {
            _emailDbContext = context;
            _userService = userService;
            _emailSender = emailSender;
        }

        [HttpGet("emails")]
        public virtual async Task<IActionResult> GetEmails()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account_id = _userService.GetAccount_Id(token);
                Console.WriteLine($"Logged-in user's AccountId: {account_id}");

                var BedrijfEigenaar_id = await _emailDbContext.Bedrijf
                    .Where(b => b.Eigenaar == account_id)
                    .Select(b => b.Id)
                    .FirstOrDefaultAsync();
                Console.WriteLine($"Retrieved bedrijf_id: {BedrijfEigenaar_id}");

                if (BedrijfEigenaar_id == Guid.Empty)
                {
                    return Unauthorized("User is not associated with any company.");
                }

                var accountEmails = await _emailDbContext.Account
                    .Join(_emailDbContext.BedrijfAccounts,
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
                // Validate email format
                if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    var errorDetails = new {
                        message = "FalseFormatEmail",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account_id = _userService.GetAccount_Id(token);
                var bedrijf_id = await _emailDbContext.Bedrijf
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

                var domein = await _emailDbContext.Bedrijf
                    .Where(b => b.Id == bedrijf_id)
                    .Select(b => b.Domein)
                    .FirstOrDefaultAsync();
                if (domein == null || !EmailHelper.CheckDomeinAllowToAddToCompany(model.Email, domein))
                {
                    var errorDetails = new {
                        message = "FalseDomein",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                var account = await _emailDbContext.Account
                    .FirstOrDefaultAsync(a => a.Email == model.Email);

                if (account == null)
                {
                    var password = Password.CreatePassword(8);
                    var newAccount = new Account
                    {
                        Id = Guid.NewGuid(),
                        Email = model.Email,
                        wachtwoord = password
                    };
                    _emailDbContext.Account.Add(newAccount);
                    await _emailDbContext.SaveChangesAsync();
                    account = await _emailDbContext.Account
                        .FirstOrDefaultAsync(a => a.Email == model.Email);
                    // string context = $"{model.Email} {password} Dit zijn uw loggin gegevens";
                    // _emailSencer.SendEmail("pbt05@hotmail.nl", "Account gegevens", context);
                }

                var bedrijf = await _emailDbContext.Bedrijf.FindAsync(bedrijf_id);

                var Abbonement = await _emailDbContext.Bedrijf
                    .Where(b => b.Id == bedrijf_id)
                    .Select(b => b.Abbonement)
                    .FirstOrDefaultAsync();
                var CountAccountBedrijf = await _emailDbContext.BedrijfAccounts
                    .Where(ab => ab.bedrijf_id == bedrijf_id)
                    .CountAsync();

                if (!EmailHelper.CheckAmountAllowedToAddToCompany(Abbonement, CountAccountBedrijf))
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

                _emailDbContext.BedrijfAccounts.Add(accountBedrijf);
                await _emailDbContext.SaveChangesAsync();

                var succesDetails = new {
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

                var account = await _emailDbContext.Account
                    .FirstOrDefaultAsync(a => a.Email == model.Email);

                if (account == null)
                {
                    var errorDetails = new {
                        message = "Account with the provided email does not exist.",
                        statusCode = 404
                    };
                    return NotFound(errorDetails);
                }

                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account_id = _userService.GetAccount_Id(token);
                var bedrijf_id = await _emailDbContext.Bedrijf
                    .Where(b => b.Eigenaar == account_id)
                    .Select(b => b.Id)
                    .FirstOrDefaultAsync();

                var BedrijfNaam = await _emailDbContext.Bedrijf
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

                var accountBedrijf = await _emailDbContext.BedrijfAccounts
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

                _emailDbContext.BedrijfAccounts.Remove(accountBedrijf);
                await _emailDbContext.SaveChangesAsync();
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
