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

        [Authorize(Policy = "BackOffice")]
        [HttpGet("getbackofficeaccounts")]
        public async Task<IActionResult> GetBackOfficeAccounts()
        {
            try
            {
                var accounts = await _accountDbContext.Account
                    .Where(a => a.Rol == "Backofficemedewerker")
                    .Select(a => new
                    {
                        a.Id,
                        a.Email,
                        a.Naam,
                        a.Adres,
                        a.TelefoonNummer
                    })
                    .ToListAsync();

                return Ok(new { accounts });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching back office accounts: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error." });
            }
        }

        [Authorize(Policy = "BackOffice")]
        [HttpPost("createbackofficeaccount")]
        public async Task<IActionResult> CreateBackOfficeAccount([FromBody] LoginRegisterModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                {
                    return BadRequest(new { message = "Invalid account creation request." });
                }

                var existingAccount = await _accountDbContext.Account
                    .FirstOrDefaultAsync(a => a.Email == model.Email);

                if (existingAccount != null)
                {
                    return Conflict(new { message = "Email is already in use." });
                }

                var newAccount = new Account
                {
                    Email = model.Email,
                    Naam = model.Naam,
                    wachtwoord = BC.EnhancedHashPassword(model.Password),
                    Rol = "Backofficemedewerker",
                    Adres = model.Address ?? "N/A",
                    TelefoonNummer = model.PhoneNumber ?? "N/A"
                };

                _accountDbContext.Account.Add(newAccount);
                await _accountDbContext.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Back office account created successfully.",
                    AccountId = newAccount.Id,
                    Role = newAccount.Rol
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during back office account creation: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [Authorize(Policy = "BackOffice")]
        [HttpPut("updatebackofficeaccount/{id}")]
        public async Task<IActionResult> UpdateBackOfficeAccount(Guid id, [FromBody] UpdateUserModel model)
        {
            try
            {
                var account = await _accountDbContext.Account.FindAsync(id);
                if (account == null)
                {
                    return NotFound(new { message = "Account not found." });
                }

                account.Naam = model.Naam;
                account.Adres = model.Adres;
                account.TelefoonNummer = model.TelefoonNummer;

                await _accountDbContext.SaveChangesAsync();

                return Ok(new { message = "Back office account updated successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating back office account: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error." });
            }
        }

        [Authorize(Policy = "BackOffice")]
        [HttpDelete("deletebackofficeaccount/{id}")]
        public async Task<IActionResult> DeleteBackOfficeAccount(Guid id)
        {
            try
            {
                var account = await _accountDbContext.Account.FindAsync(id);
                if (account == null)
                {
                    return NotFound(new { message = "Account not found." });
                }

                _accountDbContext.Account.Remove(account);
                await _accountDbContext.SaveChangesAsync();

                return Ok(new { message = "Back office account removed successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing back office account: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error." });
            }
        }
        
    }
}