using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using BC = BCrypt.Net.BCrypt;
using static Backend.Controllers.AccountController;

public class AccountControllerTest
{
    private readonly AccountController _controller;
    private readonly ApplicationDbContext _context;

    public AccountControllerTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new AccountController(_context, skipSignIn: true)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private void MockAuthentication(string email, string Rol)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, Rol)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var user = new ClaimsPrincipal(identity);

        var context = new DefaultHttpContext();
        context.User = user;

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = context
        };
    }

    [Fact]
    public async Task RegisterTest()
    {
        var account = new RegisterModel
        {
            Email = "bone.groe@email.com",
            Password = "test",
            Role = "Particuliere huurder"
        };
        
        // Test if controller works
        var result = await _controller.Register(account);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        // Test if account is added to database
        var emailInDB = _context.Account
            .FirstOrDefault(a => a.Email == account.Email);
        Assert.NotNull(emailInDB);
    }

    private void MockAuthentication(string email, object rol)
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task LoginTest_SuccessfulLoggIn()
    {
        // Arrange
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = "john.doe@email.com",
            wachtwoord = BC.EnhancedHashPassword("correctpassword"),
            Rol = "Particuliere huurder"
        };
        _context.Account.Add(account);
        await _context.SaveChangesAsync();

        var loginModel = new LoginModel
        {
            Email = account.Email,
            Password = "correctpassword"
        };

        // Act
        var result = await _controller.Login(loginModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }


    [Fact]
    public async Task LoginTest_UnsuccessfulLoggIn()
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = "jane.doe@email.com",
            wachtwoord = BC.EnhancedHashPassword("test"), // Correct password
            Rol = "Particuliere huurder"
        };
        _context.Account.Add(account);
        await _context.SaveChangesAsync();

        var login = new LoginModel
        {
            Email = account.Email,
            Password = "AnotherPassword" // Incorrect password
        };

        var result = await _controller.Login(login);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

    [Fact]
    public async Task UpdateUserTest()
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = "john.doe@email.com",
            wachtwoord = BC.EnhancedHashPassword("test"),
            Rol = "Particuliere huurder",
            Naam = "John Doe",
            Adres = "Teststraat 123",
            TelefoonNummer = "0612345678"
        };
        _context.Account.Add(account);
        await _context.SaveChangesAsync();

        var updatedAccount = new UpdateUserModel 
        {
            Email = "john.doe@email.com",
            Naam = "Jane Doe",
            Adres = "Teststraat 321",
            TelefoonNummer = "0687654321"
        };

        MockAuthentication(account.Email, account.Rol);

        // Test if controller works
        var result = await _controller.UpdateUser(updatedAccount);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        
        // Test if account is updated in database
        var updatedAccountInDB = _context.Account
            .FirstOrDefault(a => a.Email == updatedAccount.Email);
        Assert.NotNull(updatedAccountInDB);
        Assert.Equal(updatedAccount.Naam, updatedAccountInDB.Naam);
        Assert.Equal(updatedAccount.Adres, updatedAccountInDB.Adres);
        Assert.Equal(updatedAccount.TelefoonNummer, updatedAccountInDB.TelefoonNummer);
        
    }
}