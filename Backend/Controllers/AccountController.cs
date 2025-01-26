using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Entities;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Backend.Models;


namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _accountDbContext;
        private readonly bool _skipSignIn;

        public AccountController(ApplicationDbContext context, bool skipSignIn = false)
        {
            _accountDbContext = context;
            _skipSignIn = skipSignIn;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
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

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            if (!_skipSignIn)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            }


            return Ok(new {
                UserId = user.Id,
                Role = user.Rol
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.Role))
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
                    Rol = model.Role,
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
                Name = u.Naam, // Ensure name is included
                Role = u.Rol ?? "Error", // Ensure role is included
                Address = u.Adres ?? "N/A",
                PhoneNumber = u.TelefoonNummer ?? "N/A"
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
            Naam = model.Naam, // Ensure the name is set
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
            newUser.Email,
            newUser.Naam, // Ensure the name is returned
            Role = newUser.Rol,
            Address = newUser.Adres,
            PhoneNumber = newUser.TelefoonNummer
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
public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleModel model)
{
    try
    {
        var user = await _accountDbContext.Account.FindAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found." });
        }

        user.Rol = model.Role; // Use 'Role' instead of 'NewRole'
        user.Naam = model.Naam; // Ensure name is updated
        user.Email = model.Email; // Ensure email is updated
        await _accountDbContext.SaveChangesAsync();

        return Ok(new { message = "User role updated successfully.", user });
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
            public string Adres { get; set; }
            public string TelefoonNummer { get; set; }
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost("updateuser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
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

                user.Naam = model.Naam;
                user.Adres = model.Adres;
                user.TelefoonNummer = model.TelefoonNummer;

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