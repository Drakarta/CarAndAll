using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions; // Add this import
using Backend.Data;
using Backend.Entities;
using Backend.Interface;
using Backend.ConceptFiles;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly ApplicationDbContext _emailDbContext;
        private readonly IUserService _userService;
        private readonly EmailSencer _emailSencer;

        public EmailController(ApplicationDbContext context, IUserService userService, EmailSencer emailSencer)
        {
            _emailDbContext = context;
            _userService = userService;
            _emailSencer = emailSencer;
        }

         private Boolean CheckAmountAllowedToAddToCompany(int abbonement, int Emails)
    {
        Console.WriteLine(abbonement);
        if(abbonement == 1 && Emails >= 2)
        {
            return false;
        }
        else if(abbonement == 2 && Emails >= 5)
        {
            return false;
        }
        else if(abbonement == 3 && Emails >= 10)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private Boolean CheckDomeinAllowToAddToCompany(string email, string domein){

        string[] emailSplit = email.Split('@');
        Console.WriteLine(emailSplit[1]);
        Console.WriteLine(domein);
        if(emailSplit[1] != domein)
        {
            return false;
        } else {
            return true;
        }
    }

        public class EmailModel
        {
            [JsonProperty("email")]
            public required string Email { get; set; }
        }

        [HttpGet("emails")] // Ensure this matches the frontend request
        public async Task<IActionResult> GetEmails()
        {
            try
            {
                var account_id = _userService.GetAccount_Id();  // Get logged-in user's AccountId
                Console.WriteLine($"Logged-in user's AccountId: {account_id}");

                var BedrijfEigenaar_id = await _emailDbContext.Bedrijf
                    .Where(b => b.Eigenaar == account_id)
                    .Select(b => b.Id)
                    .FirstOrDefaultAsync();
                Console.WriteLine($"Retrieved bedrijf_id: {BedrijfEigenaar_id}");

                if (BedrijfEigenaar_id == 0)
                {
                    return Unauthorized("User is not associated with any company.");
                }

                var accountEmails = await _emailDbContext.Accounts
                    .Join(_emailDbContext.AccountBedrijven,
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
        public async Task<IActionResult> AddUserToCompany([FromBody] EmailModel model)
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
                 var account_id = _userService.GetAccount_Id();  // Get logged-in user's AccountId
                var bedrijf_id = await _emailDbContext.Bedrijf
                    .Where(b => b.Eigenaar == account_id)
                    .Select(b => b.Id)
                    .FirstOrDefaultAsync();

                var domein = await _emailDbContext.Bedrijf
                    .Where(b => b.Id == bedrijf_id)
                    .Select(b => b.Domein)
                    .FirstOrDefaultAsync();
                 if (domein == null || CheckDomeinAllowToAddToCompany(model.Email, domein) == false)
                {
                    var errorDetails = new {
                        message = "FalseDomein",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                var account = await _emailDbContext.Accounts
                    .FirstOrDefaultAsync(a => a.Email == model.Email);

                if (account == null)
                {
                    var password = Password.CreatePassword(8);
                    var newAccount = new Account
                    {
                        Id = Guid.NewGuid(),
                        Email = model.Email,
                        wachtwoord = password // Assuming the Account entity has a Password property
                    };
                    _emailDbContext.Accounts.Add(newAccount);
                    await _emailDbContext.SaveChangesAsync();
                    account = await _emailDbContext.Accounts
                        .FirstOrDefaultAsync(a => a.Email == model.Email);
                    string context = (model.Email + " " + password + "Dit zijn uw loggin gegevens");
                    _emailSencer.SendEmail("pbt05@hotmail.nl", "Account gegevens", context);
                }

               

                var bedrijf = await _emailDbContext.Bedrijf.FindAsync(bedrijf_id);

                var Abbonement = await _emailDbContext.Bedrijf
                    .Where(b => b.Id == bedrijf_id)
                    .Select(b => b.Abbonement)
                    .FirstOrDefaultAsync();
                var CountAccountBedrijf = await _emailDbContext.AccountBedrijven
                    .Where(ab => ab.bedrijf_id == bedrijf_id)
                    .CountAsync();

                
                Console.WriteLine(domein);

                if (CheckAmountAllowedToAddToCompany(Abbonement, CountAccountBedrijf) == false)
                {
                    var errorDetails = new {
                        message = "MaxNumber",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }
               

                if (bedrijf_id == 0)
                {
                    var errorDetails = new {
                        message = "User is not associated with any company.",
                        statusCode = 401
                    };
                    return Unauthorized(errorDetails);
                }

                var accountBedrijf = new AccountBedrijf
                {
                    account_id = account.Id,
                    bedrijf_id = bedrijf_id,
                    Account = account,
                    Bedrijf = bedrijf ?? throw new InvalidOperationException("Bedrijf not found")
                };

                _emailDbContext.AccountBedrijven.Add(accountBedrijf);
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
        public async Task<IActionResult> RemoveUserFromCompany([FromBody] EmailModel model)
        {
            try
            {
                // Validate email format
                if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    var errorDetails = new {
                        message = "This is not the correct format of an email.",
                        statusCode = 400
                    };
                    return BadRequest(errorDetails);
                }

                var account = await _emailDbContext.Accounts
                    .FirstOrDefaultAsync(a => a.Email == model.Email);

                if (account == null)
                {
                    var errorDetails = new {
                        message = "Account with the provided email does not exist.",
                        statusCode = 404
                    };
                    return NotFound(errorDetails);
                }

                var account_id = _userService.GetAccount_Id();  // Get logged-in user's AccountId
                var bedrijf_id = await _emailDbContext.Bedrijf
                    .Where(b => b.Eigenaar == account_id)
                    .Select(b => b.Id)
                    .FirstOrDefaultAsync();

                if (bedrijf_id == 0)
                {
                    var errorDetails = new {
                        message = "User is not associated with any company.",
                        statusCode = 401
                    };
                    return Unauthorized(errorDetails);
                }

                var accountBedrijf = await _emailDbContext.AccountBedrijven
                    .FirstOrDefaultAsync(ab => ab.account_id == account.Id && ab.bedrijf_id == bedrijf_id);

                if (accountBedrijf == null)
                {
                    var errorDetails = new {
                        message = "User is not associated with company.",
                        statusCode = 404
                    };
                    return NotFound(errorDetails);
                }

                _emailDbContext.AccountBedrijven.Remove(accountBedrijf);
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