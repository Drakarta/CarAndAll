using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;

namespace CarAndAll
{
    public class EmailDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Bedrijf> Bedrijven { get; set; }
        public DbSet<AccountBedrijf> AccountBedrijven { get; set; } // Update this line

        public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define the many-to-many relationship between Account and Bedrijf
            modelBuilder.Entity<AccountBedrijf>()
                .HasKey(ab => new { ab.account_id, ab.bedrijf_id });

            modelBuilder.Entity<AccountBedrijf>()
                .HasOne(ab => ab.Account)
                .WithMany(a => a.AccountBedrijven)
                .HasForeignKey(ab => ab.account_id);

            modelBuilder.Entity<AccountBedrijf>()
                .HasOne(ab => ab.Bedrijf)
                .WithMany(b => b.AccountBedrijven)
                .HasForeignKey(ab => ab.bedrijf_id);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly ApplicationDbContext _emailDbContext;
        private readonly IUserService _userService;

        public EmailController(ApplicationDbContext context, IUserService userService)
        {
            _emailDbContext = context;
            _userService = userService;
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
                var bedrijf_id = await _emailDbContext.AccountBedrijven
                    .Where(ab => ab.account_id == account_id)
                    .Select(ab => ab.bedrijf_id)
                    .FirstOrDefaultAsync();

                if (bedrijf_id == 0)
                {
                    return Unauthorized("User is not associated with any company.");
                }

                var accountEmails = await _emailDbContext.Accounts
                    .Join(_emailDbContext.AccountBedrijven,
                          a => a.Id,
                          ab => ab.account_id,
                          (a, ab) => new { a.Email, ab.bedrijf_id })
                    .Where(result => result.bedrijf_id == bedrijf_id)
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
                var account = await _emailDbContext.Accounts
                    .FirstOrDefaultAsync(a => a.Email == model.Email);

                if (account == null)
                {
                    return NotFound("Account with the provided email does not exist.");
                }

                var account_id = _userService.GetAccount_Id();  // Get logged-in user's AccountId
                var bedrijf_id = await _emailDbContext.AccountBedrijven
                    .Where(ab => ab.account_id == account_id)
                    .Select(ab => ab.bedrijf_id)
                    .FirstOrDefaultAsync();

                if (bedrijf_id == 0)
                {
                    return Unauthorized("User is not associated with any company.");
                }

                var accountBedrijf = new AccountBedrijf
                {
                    account_id = account.Id,
                    bedrijf_id = bedrijf_id
                };

                _emailDbContext.AccountBedrijven.Add(accountBedrijf);
                await _emailDbContext.SaveChangesAsync();

                return Ok("User added to the company successfully.");
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
                var account = await _emailDbContext.Accounts
                    .FirstOrDefaultAsync(a => a.Email == model.Email);

                if (account == null)
                {
                    return NotFound("Account with the provided email does not exist.");
                }

                var account_id = _userService.GetAccount_Id();  // Get logged-in user's AccountId
                var bedrijf_id = await _emailDbContext.AccountBedrijven
                    .Where(ab => ab.account_id == account_id)
                    .Select(ab => ab.bedrijf_id)
                    .FirstOrDefaultAsync();

                if (bedrijf_id == 0)
                {
                    return Unauthorized("User is not associated with any company.");
                }

                var accountBedrijf = await _emailDbContext.AccountBedrijven
                    .FirstOrDefaultAsync(ab => ab.account_id == account.Id && ab.bedrijf_id == bedrijf_id);

                if (accountBedrijf == null)
                {
                    return NotFound("User is not associated with the company.");
                }

                _emailDbContext.AccountBedrijven.Remove(accountBedrijf);
                await _emailDbContext.SaveChangesAsync();

                return Ok("User removed from the company successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}