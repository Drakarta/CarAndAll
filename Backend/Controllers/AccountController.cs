using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Entities;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;
using System.Security.Claims;
using System.Threading.Tasks;


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

            var Role = await _accountDbContext.Account
                .Where(account => account.Email == model.Email)
                .Select(account => account.Rol)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            if (!BC.EnhancedVerify(model.Password, user.wachtwoord))
            {
                return Unauthorized(new { message = "Invalid email or password." });

            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Rol)
            };

            Console.WriteLine($"Assigned Role: {user.Rol}");

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);


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
                    wachtwoord = BC.EnhancedHashPassword(model.Password),
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
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("getuserbyid")]
        public async Task<IActionResult> GetUserById()
        {
         
            
             try
            {
                var accountEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (accountEmail == null)
                {
                    return BadRequest(new { message = "Email claim not found." });
                }

                var user = await _accountDbContext.Account
                    .FirstOrDefaultAsync(a => a.Email == accountEmail);

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

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {

                Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                Response.Headers["Pragma"] = "no-cache";
                Response.Headers["Expires"] = "0";


                Response.Cookies.Delete(".AspNetCore.Identity.Application", new CookieOptions
                {
                    Path = "/"
                });


                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return Ok(new { message = "User logged out successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout Error: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred during logout." });
            }
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _accountDbContext.Account
                    .Select(u => new 
                    {
                        u.Id,
                        u.Email,
                        Rol = u.Rol ?? "Error",
                        Adres = u.Adres ?? "N/A",
                        TelefoonNummer = u.TelefoonNummer ?? "N/A"
                    })
                    .ToListAsync();

                return Ok(new { users });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error." });
            }
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> AddUser([FromBody] LoginRegisterModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                {
                    return BadRequest(new { message = "Invalid user creation request." });
                }

                var existingUser = await _accountDbContext.Account
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (existingUser != null)
                {
                    return Conflict(new { message = "Email is already in use." });
                }

                var newUser = new Account
                {
                    Id = Guid.NewGuid(), // Ensure a unique identifier is generated
                    Email = model.Email,
                    Naam = model.Naam,
                    wachtwoord = BC.EnhancedHashPassword(model.Password),
                    Rol = model.Role ?? "DefaultRole", // Set a role if provided or default to a fallback
                    Adres = model.Address ?? "N/A",
                    TelefoonNummer = model.PhoneNumber ?? "N/A"
                };

                _accountDbContext.Account.Add(newUser);
                await _accountDbContext.SaveChangesAsync();

                return Ok(new
                {
                    Message = "User created successfully.",
                    UserId = newUser.Id,
                    Role = newUser.Rol
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during user creation: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] string newRole)
        {
            try
            {
                var user = await _accountDbContext.Account.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                user.Rol = newRole;
                await _accountDbContext.SaveChangesAsync();

                return Ok(new { message = "User role updated successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user role: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error." });
            }
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> RemoveUser(Guid id)
        {
            try
            {
                var user = await _accountDbContext.Account.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                _accountDbContext.Account.Remove(user);
                await _accountDbContext.SaveChangesAsync();

                return Ok(new { message = "User removed successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing user: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error." });
            }
        }
        public class UpdateUserModel
        {
            public string Email { get; set; }
            public string Naam { get; set; }
            public string Address { get; set; }
            public string PhoneNumber { get; set; }
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost("updateuser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
        {
            try
            {
                var accountEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                Console.WriteLine($"{model.Email} {model.Naam} {model.Address} {model.PhoneNumber}");
                if (accountEmail == null)
                {
                    return BadRequest(new { message = "Email claim not found." });
                }

                var user = await _accountDbContext.Account
                    .FirstOrDefaultAsync(a => a.Email == accountEmail);

                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                user.Naam = model.Naam;
                user.Adres = model.Address;
                user.TelefoonNummer = model.PhoneNumber;

                await _accountDbContext.SaveChangesAsync();

                return Ok(new
                {
                    Message = "User updated successfully.",
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
                Console.WriteLine($"Error updating user: {ex.Message}");
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }
        
    }
}