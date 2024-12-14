using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Entities;

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
            if (model.Password != user.Wachtwoord)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            return Ok(new { UserId = user.Id });
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

            // Check if email is already in use
            var existingUser = await _accountDbContext.Account
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (existingUser != null)
            {
                return Conflict(new { message = "Email is already in use." });
            }

            var newUser = new Account
            {
                // Id = Guid.NewGuid(),
                Email = model.Email,
                Wachtwoord = model.Password // Ideally, you should hash the password before storing it.
            };

            // Save user to database
            _accountDbContext.Account.Add(newUser);
            await _accountDbContext.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }
        catch (Exception ex)
        {
            // Log the exception for debugging (use your logger here)
            Console.WriteLine($"Error occurred during registration: {ex.Message}");
            
            // Return the error message
            return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
        }
    }}
}
