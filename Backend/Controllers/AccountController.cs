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

        // POST: api/account/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Check if the request body is empty
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new { message = "Invalid login request." });
            }
            
            // Check if the user exists in the database
            var user = await _accountDbContext.Account
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            // Check if the user exists and if the password is correct
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

            // Create a list of claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Rol)
            };

            // Create a claims identity and principal
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Sign in the user if the skip sign-in flag is not set
            if (!_skipSignIn)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            }

            // Return the user's ID and role
            return Ok(new {
                UserId = user.Id,
                Role = user.Rol
            });
        }

        // POST: api/account/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                // Check if the request body is empty
                if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.Role))
                {
                    return BadRequest(new { message = "Invalid registration request." });
                }

                // Check if the user already exists in the database
                var existingUser = await _accountDbContext.Account
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                // Check if the user already exists
                if (existingUser != null)
                {
                    return Conflict(new { message = "Email is already in use." });
                }

                // Create a new user
                var newUser = new Account
                {
                    Email = model.Email,
                    wachtwoord = BC.EnhancedHashPassword(model.Password),
                    Rol = model.Role,
                };

                // Add the new user to the database
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

                // Return a success message
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
        // POST: api/account/getuserbyid
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
                // Retrieve the current user's email based on the logged-in session or claims
                var accountEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (accountEmail == null)
                {
                    return BadRequest(new { message = "Email claim not found." });
                }

                // Retrieve the user from the database
                var user = await _accountDbContext.Account
                    .FirstOrDefaultAsync(a => a.Email == accountEmail);

                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                // Return the user's ID, email, name, address, phone number, and role
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

        // POST: api/account/logout
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Clear the response headers
                Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                Response.Headers["Pragma"] = "no-cache";
                Response.Headers["Expires"] = "0";

                // Sign out the user
                Response.Cookies.Delete(".AspNetCore.Identity.Application", new CookieOptions
                {
                    Path = "/"
                });
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Return a success message
                return Ok(new { message = "User logged out successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout Error: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred during logout." });
            }
        }

        // GET: api/account/users
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                // Retrieve all users from the database
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

                // Return the list of users
                return Ok(new { users });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error." });
            }
        }

        // POST: api/account/CreateUser
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> AddUser([FromBody] LoginRegisterModel model)
        {
            try
            {
                // Check if the request body is empty
                if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                {
                    return BadRequest(new { message = "Invalid user creation request." });
                }

                // Check if the user already exists in the database
                var existingUser = await _accountDbContext.Account
                    .FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    return Conflict(new { message = "Email is already in use." });
                }

                // Create a new user
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

                // Return a success message
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

        // PUT: api/account/users/{id}
        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleModel model)
        {
            try
            {
                // Check if the request body is empty
                var user = await _accountDbContext.Account.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                // Update the user's role
                user.Rol = model.Role;
                user.Naam = model.Naam;
                user.Email = model.Email;
                await _accountDbContext.SaveChangesAsync();

                // Return a success message
                return Ok(new { message = "User role updated successfully.", user });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user role: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error." });
            }
        }
        
        // DELETE: api/account/users/{id}
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> RemoveUser(Guid id)
        {
            try
            {
                // Check if the user exists in the database
                var user = await _accountDbContext.Account.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                // Remove the user from the database
                _accountDbContext.Account.Remove(user);
                await _accountDbContext.SaveChangesAsync();

                // Return a success message
                return Ok(new { message = "User removed successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing user: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error." });
            }
        }

        // POST: api/account/updateuser
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
                // Check if the request body is empty
                var accountEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (accountEmail == null)
                {
                    return BadRequest(new { message = "Email claim not found." });
                }

                // Retrieve the user from the database
                var user = await _accountDbContext.Account
                    .FirstOrDefaultAsync(a => a.Email == accountEmail);

                // Check if the user exists
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                // Update the user's details
                user.Naam = model.Naam;
                user.Adres = model.Adres;
                user.TelefoonNummer = model.TelefoonNummer;
                await _accountDbContext.SaveChangesAsync();

                // Return a success message
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

        // GET: api/account/getbackofficeaccounts
        [Authorize(Policy = "BackOffice")]
        [HttpGet("getbackofficeaccounts")]
        public async Task<IActionResult> GetBackOfficeAccounts()
        {
            try
            {
                // Retrieve all back office accounts from the database
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
                
                // Return the list of back office accounts
                return Ok(new { accounts });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching back office accounts: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error." });
            }
        }

        // POST: api/account/createbackofficeaccount
        [Authorize(Policy = "BackOffice")]
        [HttpPost("createbackofficeaccount")]
        public async Task<IActionResult> CreateBackOfficeAccount([FromBody] LoginRegisterModel model)
        {
            try
            {
                // Check if the request body is empty
                if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                {
                    return BadRequest(new { message = "Invalid account creation request." });
                }

                // Check if the account already exists in the database
                var existingAccount = await _accountDbContext.Account
                    .FirstOrDefaultAsync(a => a.Email == model.Email);
                if (existingAccount != null)
                {
                    return Conflict(new { message = "Email is already in use." });
                }

                // Create a new back office account
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

                // Return a success message
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

        // PUT: api/account/updatebackofficeaccount/{id}
        [Authorize(Policy = "BackOffice")]
        [HttpPut("updatebackofficeaccount/{id}")]
        public async Task<IActionResult> UpdateBackOfficeAccount(Guid id, [FromBody] UpdateUserModel model)
        {
            try
            {
                // Check if the request body is empty
                var account = await _accountDbContext.Account.FindAsync(id);
                if (account == null)
                {
                    return NotFound(new { message = "Account not found." });
                }

                // Update the back office account details
                account.Naam = model.Naam;
                account.Adres = model.Adres;
                account.TelefoonNummer = model.TelefoonNummer;
                await _accountDbContext.SaveChangesAsync();

                // Return a success message
                return Ok(new { message = "Back office account updated successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating back office account: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error." });
            }
        }

        // DELETE: api/account/deletebackofficeaccount/{id}
        [Authorize(Policy = "BackOffice")]
        [HttpDelete("deletebackofficeaccount/{id}")]
        public async Task<IActionResult> DeleteBackOfficeAccount(Guid id)
        {
            try
            {
                // Check if the back office account exists in the database
                var account = await _accountDbContext.Account.FindAsync(id);
                if (account == null)
                {
                    return NotFound(new { message = "Account not found." });
                }

                // Remove the back office account from the database
                _accountDbContext.Account.Remove(account);
                await _accountDbContext.SaveChangesAsync();

                // Return a success message
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