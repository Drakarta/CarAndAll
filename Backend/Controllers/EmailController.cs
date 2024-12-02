using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CarAndAll
{
    public class Email
    {
        public int Id { get; set; }
        public required string Address { get; set; }
    }

    public class EmailDbContext : DbContext
    {
        public required DbSet<Email> Emails { get; set; }

        public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options)
        {
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly EmailDbContext _emailDbContext;

        public EmailController(EmailDbContext context)
        {
            _emailDbContext = context;
        }

        // ...existing code...

        public class EmailModel
        {
            [JsonProperty("Email")]
            public required string Email { get; set; }
        }

        [HttpGet("emails")]
        public async Task<IActionResult> GetEmails()
        {
            try
            {
                var emails = await _emailDbContext.Emails.Select(e => e.Address).ToListAsync();
                return Ok(emails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("emails/add")]
        public async Task<IActionResult> AddEmail([FromBody] EmailModel emailModel)
        {
            try
            {
                _emailDbContext.Emails.Add(new Email { Address = emailModel.Email });
                await _emailDbContext.SaveChangesAsync();
                var emails = await _emailDbContext.Emails.Select(e => e.Address).ToListAsync();
                return Ok(emails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("emails/{email}")]
        public async Task<IActionResult> RemoveEmail(string email)
        {
            try
            {
                var emailEntity = await _emailDbContext.Emails.FirstOrDefaultAsync(e => e.Address == email);
                if (emailEntity != null)
                {
                    _emailDbContext.Emails.Remove(emailEntity);
                    await _emailDbContext.SaveChangesAsync();
                }
                var emails = await _emailDbContext.Emails.Select(e => e.Address).ToListAsync();
                return Ok(emails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}