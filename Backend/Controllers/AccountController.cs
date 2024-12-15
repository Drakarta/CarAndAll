using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Entities;
using BC = BCrypt.Net.BCrypt;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _accountDbContext;

        public AccountController(ApplicationDbContext context)
        {
            _accountDbContext = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRegisterModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new { message = "Invalid login request." });
            }
            var user = await _accountDbContext.Account
                .FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            if (!BC.EnhancedVerify(model.Password, user.Wachtwoord))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            return Ok(new { 
                UserId = user.Id,
                Role = user.Rol
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginRegisterModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                {
                    return BadRequest(new { message = "Invalid registration request." });
                }

                var existingUser = await _accountDbContext.Account
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (existingUser != null)
                {
                    return Conflict(new { message = "Email is already in use." });
                }

                var newUser = new Account
                {
                    Email = model.Email,
                    Wachtwoord = BC.EnhancedHashPassword(model.Password),
                    Rol = "Particuliere huurder"
                };

                _accountDbContext.Account.Add(newUser);
                await _accountDbContext.SaveChangesAsync();
                
                var user = _accountDbContext.Account
                    .Where(account => account.Email == model.Email)
                    .Select(account => new 
                    { 
                        account.Id, 
                        account.Rol 
                    })
                    .FirstOrDefault();

                return Ok(new { 
                    Message = "User registered successfully.",
                    UserId = user.Id,
                    Role = user.Rol
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during registration: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        public class IdModel
        {
            public required string Id { get; set; }
        }

        [HttpPost("getuserbyid")]
        public async Task<IActionResult> GetUserById([FromBody] IdModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Id))
            {
                return BadRequest(new { message = "Invalid request: Id is required." });
            }

            if (!Guid.TryParse(model.Id, out var userId))
            {
                return BadRequest(new { message = "Invalid request: Id must be a valid GUID." });
            }

            try
            {
                var user = await _accountDbContext.Account
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                return Ok(new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Name = user.Naam,
                    Address = user.Adres,
                    PhoneNumber = user.TelefoonNummer,
                    Role = user.Rol
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user by ID: {ex.Message}");
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}
