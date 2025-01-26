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

    [Fact]
    public async Task GetBackOfficeAccountsTest()
    {
        // Clear the database
        _context.Account.RemoveRange(_context.Account);
        await _context.SaveChangesAsync();

        // Arrange
        var backOfficeAccount1 = new Account
        {
            Id = Guid.NewGuid(),
            Email = "backoffice1@email.com",
            wachtwoord = BC.EnhancedHashPassword("password1"),
            Rol = "Backofficemedewerker",
            Naam = "Back Office User 1",
            Adres = "Address 1",
            TelefoonNummer = "0612345678"
        };

        var backOfficeAccount2 = new Account
        {
            Id = Guid.NewGuid(),
            Email = "backoffice2@email.com",
            wachtwoord = BC.EnhancedHashPassword("password2"),
            Rol = "Backofficemedewerker",
            Naam = "Back Office User 2",
            Adres = "Address 2",
            TelefoonNummer = "0687654321"
        };

        _context.Account.Add(backOfficeAccount1);
        _context.Account.Add(backOfficeAccount2);
        await _context.SaveChangesAsync();

        MockAuthentication("admin@email.com", "Backofficemedewerker");

        // Act
        var result = await _controller.GetBackOfficeAccounts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        Assert.NotNull(okResult.Value);
        var accountsProperty = okResult.Value.GetType().GetProperty("accounts");
        Assert.NotNull(accountsProperty);
        var accounts = Assert.IsAssignableFrom<IEnumerable<object>>(accountsProperty.GetValue(okResult.Value));
        Assert.Equal(2, accounts.Count());
    }

    [Fact]
    public async Task CreateBackOfficeAccountTest_Success()
    {
        // Arrange
        var model = new LoginRegisterModel
        {
            Email = "backoffice.new@email.com",
            Password = "newpassword",
            Naam = "New Back Office User",
            Address = "New Address",
            PhoneNumber = "0612345678"
        };

        MockAuthentication("admin@email.com", "Backofficemedewerker");

        // Act
        var result = await _controller.CreateBackOfficeAccount(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var createdAccount = _context.Account.FirstOrDefault(a => a.Email == model.Email);
        Assert.NotNull(createdAccount);
        Assert.Equal(model.Naam, createdAccount.Naam);
        Assert.Equal(model.Address, createdAccount.Adres);
        Assert.Equal(model.PhoneNumber, createdAccount.TelefoonNummer);
        Assert.Equal("Backofficemedewerker", createdAccount.Rol);
    }

    [Fact]
    public async Task CreateBackOfficeAccountTest_EmailAlreadyInUse()
    {
        // Arrange
        var existingAccount = new Account
        {
            Id = Guid.NewGuid(),
            Email = "backoffice.existing@email.com",
            wachtwoord = BC.EnhancedHashPassword("existingpassword"),
            Rol = "Backofficemedewerker",
            Naam = "Existing Back Office User",
            Adres = "Existing Address",
            TelefoonNummer = "0612345678"
        };
        _context.Account.Add(existingAccount);
        await _context.SaveChangesAsync();

        var model = new LoginRegisterModel
        {
            Email = existingAccount.Email,
            Password = "newpassword",
            Naam = "New Back Office User",
            Address = "New Address",
            PhoneNumber = "0612345678"
        };

        MockAuthentication("admin@email.com", "Backofficemedewerker");

        // Act
        var result = await _controller.CreateBackOfficeAccount(model);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(409, conflictResult.StatusCode);
    }

    [Fact]
    public async Task CreateBackOfficeAccountTest_InvalidRequest()
    {
        // Arrange
        var model = new LoginRegisterModel
        {
            Email = "",
            Password = "",
            Naam = "New Back Office User",
            Address = "New Address",
            PhoneNumber = "0612345678"
        };

        MockAuthentication("admin@email.com", "Backofficemedewerker");

        // Act
        var result = await _controller.CreateBackOfficeAccount(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task UpdateBackOfficeAccountTest_Success()
    {
        // Arrange
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = "backoffice.update@email.com",
            wachtwoord = BC.EnhancedHashPassword("password"),
            Rol = "Backofficemedewerker",
            Naam = "Old Name",
            Adres = "Old Address",
            TelefoonNummer = "0612345678"
        };
        _context.Account.Add(account);
        await _context.SaveChangesAsync();

        var updateModel = new UpdateUserModel
        {
            Naam = "New Name",
            Adres = "New Address",
            TelefoonNummer = "0687654321"
        };

        MockAuthentication("admin@email.com", "Backofficemedewerker");

        // Act
        var result = await _controller.UpdateBackOfficeAccount(account.Id, updateModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var updatedAccount = _context.Account.FirstOrDefault(a => a.Id == account.Id);
        Assert.NotNull(updatedAccount);
        Assert.Equal(updateModel.Naam, updatedAccount.Naam);
        Assert.Equal(updateModel.Adres, updatedAccount.Adres);
        Assert.Equal(updateModel.TelefoonNummer, updatedAccount.TelefoonNummer);
    }

    [Fact]
    public async Task UpdateBackOfficeAccountTest_AccountNotFound()
    {
        // Arrange
        var updateModel = new UpdateUserModel
        {
            Naam = "New Name",
            Adres = "New Address",
            TelefoonNummer = "0687654321"
        };

        MockAuthentication("admin@email.com", "Backofficemedewerker");

        // Act
        var result = await _controller.UpdateBackOfficeAccount(Guid.NewGuid(), updateModel);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task DeleteBackOfficeAccountTest_Success()
    {
        // Arrange
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = "backoffice.delete@email.com",
            wachtwoord = BC.EnhancedHashPassword("password"),
            Rol = "Backofficemedewerker",
            Naam = "Back Office User",
            Adres = "Address",
            TelefoonNummer = "0612345678"
        };
        _context.Account.Add(account);
        await _context.SaveChangesAsync();

        MockAuthentication("admin@email.com", "Backofficemedewerker");

        // Act
        var result = await _controller.DeleteBackOfficeAccount(account.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var deletedAccount = _context.Account.FirstOrDefault(a => a.Id == account.Id);
        Assert.Null(deletedAccount);
    }
}