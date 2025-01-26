using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Entities;
using Backend.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Backend.Services;

namespace Backend.test {
public class WagenparkbeheerderControllerTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly ApplicationDbContext _context;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private readonly WagenparkbeheerderController _controller;
    

    public WagenparkbeheerderControllerTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "WagenparkbeheerDatabase")
             .LogTo(Console.WriteLine, LogLevel.Information)
            .Options;
        _context = new ApplicationDbContext(_options);

        _mockEmailSender = new Mock<IEmailSender>();
        var mockVerhuurAanvraagService = new Mock<VerhuurAanvraagService>(_context);
        _controller = new WagenparkbeheerderController(_context, _mockEmailSender.Object, mockVerhuurAanvraagService.Object);

    }

    [Fact]
    public async Task GetEmails_ReturnsOk_WhenAuthorized()
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = "WagenParkBeheerGetEmails@example.com",
            wachtwoord = "hashed_password",
            Rol = "Wagenparkbeheerder",
            BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>(),
        };

        var werknemer = new Account
        {
            Id = Guid.NewGuid(),
            Email = "werknemerGetEmails@example.com",
            wachtwoord = "hashed_password",
            Rol = "Zakelijke Klant",
            BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>(),
        };

        var abonnement = new Abonnement
        {
            Id = 2,
            Naam = "Test Abonnement",
            Prijs_multiplier = 1.5,
            Beschrijving = "Test Beschrijving",
            Max_medewerkers = 10,
        };

        var bedrijf = new Bedrijf
        {
            Id = Guid.NewGuid(),
            Eigenaar = account.Id,
            AbonnementId = abonnement.Id,
            Domein = "example.com",
            BedrijfAccounts = new List<BedrijfAccounts>(),
            BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>(),
        };

        var BedrijfAccounts = new BedrijfAccounts
        {
            account_id = werknemer.Id,
            bedrijf_id = bedrijf.Id,
            Account = account,
            Bedrijf = bedrijf,
        };

        var wagenparkbeheerders = new BedrijfWagenparkbeheerders
        {
            account_id = account.Id,
            bedrijf_id = bedrijf.Id,
            Account = account,
            Bedrijf = bedrijf,
        };

        _context.Abonnement.Add(abonnement);
        _context.Account.Add(account);
        _context.Bedrijf.Add(bedrijf);
        _context.BedrijfAccounts.Add(BedrijfAccounts);
        _context.BedrijfWagenparkbeheerders.Add(wagenparkbeheerders);
        await _context.SaveChangesAsync();

        MockAuthentication(account.Email, account.Rol);

        // Act: Calling the controller method
        var result = await _controller.GetEmails();

        // Assert: Ensure the result is Ok
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
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
    public async Task AddAccountToCompany_ReturnsOk_WhenAuthorized()
    {
        var accountId = Guid.NewGuid();
        var bedrijfId = Guid.NewGuid();

        var account = new Account
        {
            Id = accountId,
            Email = "agenparkbeheerderAddUserOkAuthroized@example.com",
            wachtwoord = "securePassword123",
            Rol = "Wagenparkbeheerder",
            BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>()
        };


        var bedrijf = new Bedrijf
        {
            Id = bedrijfId,
            Eigenaar = accountId,
            AbonnementId = 2,
            Domein = "example.com",
            BedrijfAccounts = new List<BedrijfAccounts>(),
            BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>()
        };

        var wagenparkbeheerders = new BedrijfWagenparkbeheerders
        {
            account_id = accountId,
            Account = account ?? throw new InvalidOperationException("Account not found"),
            bedrijf_id = bedrijfId,
            Bedrijf = bedrijf ?? throw new InvalidOperationException("Bedrijf not found")
        };


        _context.Account.Add(account);
        _context.BedrijfWagenparkbeheerders.Add(wagenparkbeheerders);
        _context.Bedrijf.Add(bedrijf);
        await _context.SaveChangesAsync();


        MockAuthentication(account.Email, account.Rol);

        // Act
        var emailModelAdd = new EmailModelAdd { Email = "addedUserReturnsOkWhenAuthorized@example.com", Role = "Zakelijke Klant" };
        var result = await _controller.AddAccountToCompany(emailModelAdd);


        // Assert

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task AddUserToCompany_ReturnsBadRequest_WhenEmailFormatIsInvalid()
    {
        
        var accountId = Guid.NewGuid();
        var bedrijfId = Guid.NewGuid();

        var account = new Account
        {
            Id = accountId,
            Email = "wagenparkbeheerderReturnsBadRequestEmailInvalid@example.com",
            wachtwoord = "securePassword123",
            Rol = "Wagenparkbeheerder"

        };
        var bedrijf = new Bedrijf
        {
            Id = bedrijfId,
            Eigenaar = accountId,
            AbonnementId = 2,
            Domein = "example.com",
            BedrijfAccounts = new List<BedrijfAccounts>(),
            BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>()
        };

        _context.Account.Add(account);
        _context.Bedrijf.Add(bedrijf);
        await _context.SaveChangesAsync();

        MockAuthentication(account.Email, account.Rol);

        // Act
        var emailModel = new EmailModelAdd { Email = "invalid-email-format", Role = "Zakelijke Klant" };
        var result = await _controller.AddAccountToCompany(emailModel);


        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Contains("FalseFormatEmail", badRequestResult.Value.ToString());
    }

    [Fact]
    public async Task AddAccountToCompany_ReturnsBadRequest_WhenUserAddsSelf()
    {
        var accountId = Guid.NewGuid();

        var account = new Account
        {
            Id = accountId,
            Email = "wagenparkbeheerder@example.com",
            wachtwoord = "securePassword123",
            Rol = "Wagenparkbeheerder"
        };
        var bedrijf = new Bedrijf
        {
            Eigenaar = accountId,
            AbonnementId = 2,
            Domein = "example.com",
            BedrijfAccounts = new List<BedrijfAccounts>(),
            BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>()

        };

        _context.Account.Add(account);
        _context.Bedrijf.Add(bedrijf);
        await _context.SaveChangesAsync();


        MockAuthentication(account.Email, account.Rol);

        // Act
        var emailModel = new EmailModelAdd { Email = "wagenparkbeheerder@example.com", Role = "Zakelijke Klant" };
        var result = await _controller.AddAccountToCompany(emailModel);


        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Contains("User cannot add himself/herself to the company.", badRequestResult.Value.ToString());
    }




    [Fact]
    public async Task AddAccountToCompany_ReturnsBadRequest_WhenDomainIsInvalid()
    {
        var accountId = Guid.NewGuid();

        var account = new Account
        {
            Id = accountId,
            Email = "wagenparkbeheerder@test.com",
            wachtwoord = "securePassword123",
            Rol = "Wagenparkbeheerder",
            BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>()
        };
        var bedrijf = new Bedrijf
        {
            Id = Guid.NewGuid(),
            Eigenaar = accountId,
            AbonnementId = 2,
            Domein = "test.com",
            BedrijfAccounts = new List<BedrijfAccounts>(),
            BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>()
        };
        var wagenparkbeheerders = new BedrijfWagenparkbeheerders
        {
            account_id = accountId,
            Account = account ?? throw new InvalidOperationException("Account not found"),
            bedrijf_id = bedrijf.Id,
            Bedrijf = bedrijf ?? throw new InvalidOperationException("Bedrijf not found")
        };

        _context.Account.Add(account);
        _context.Bedrijf.Add(bedrijf);
        _context.BedrijfWagenparkbeheerders.Add(wagenparkbeheerders);
        await _context.SaveChangesAsync();


        MockAuthentication(account.Email, account.Rol);


        var emailModel = new EmailModelAdd { Email = "test@invalid.com", Role = "Zakelijke Klant" };
        var result = await _controller.AddAccountToCompany(emailModel);



        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Contains("FalseDomein", badRequestResult.Value.ToString());
    }

    [Fact]
    public async Task GetVoertuigenPerUser_ReturnsOk_WhenValidEmailAndWholeYear()
    {
    var accountId = Guid.NewGuid();

    var account = new Account
    {
        Id = accountId,
        Email = "getvoertuigenreturnsokemailandwholeyear@wagenpark.com",
        wachtwoord = "securePassword123",
        Rol = "Wagenparkbeheerder",
        BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>()
    };
    var bedrijf = new Bedrijf
    {
        Id = Guid.NewGuid(),
        Eigenaar = accountId,
        AbonnementId = 2,
        Domein = "wagenpark.com",
        BedrijfAccounts = new List<BedrijfAccounts>(),
        BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>()
    };
    var wagenparkbeheerders = new BedrijfWagenparkbeheerders
    {
        account_id = accountId,
        Account = account ?? throw new InvalidOperationException("Account not found"),
        bedrijf_id = bedrijf.Id,
        Bedrijf = bedrijf ?? throw new InvalidOperationException("Bedrijf not found")
    };


    var testaccount = new Account {
        Id = Guid.NewGuid(),
        Email = "thetestemail@wagenpark.com",
        wachtwoord = "securePassword123",
        Rol = "Zakelijke Klant",
        BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>()
    };

    var auto = new Voertuig
    {
        Merk = "BMW",
        Type = "X5",
        Kenteken = "AB-CD-12",
        Kleur = "Zwart",
        Aanschafjaar = "2022",
        voertuig_categorie = "SUV",
        Status = "Beschikbaar",
    };

    var verhuurAanvraag = new VerhuurAanvraag {
        AanvraagID = 998,
        Status = "geaccepteerd",
        Startdatum = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        VoertuigID = auto.VoertuigID,
        Bestemming = "Rotterdam",
        Account = testaccount,
    };

    _context.Account.Add(account);
    _context.Account.Add(testaccount);
    _context.Bedrijf.Add(bedrijf);
    _context.Voertuigen.Add(auto);
    _context.VerhuurAanvragen.Add(verhuurAanvraag);
    _context.BedrijfWagenparkbeheerders.Add(wagenparkbeheerders);
    await _context.SaveChangesAsync();

    MockAuthentication(account.Email, account.Rol);

    var model = new VoertuigUserModel { Email = testaccount.Email, maand = "Whole year", jaar = 2023 };
    var result = await _controller.GetVoertuigenPerUser(model);
        Console.WriteLine(result);
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.Equal(200, okResult.StatusCode);
}

    [Fact]
    public async Task GetVoertuigenPerUser_ReturnsBadRequest_WhenInvalidEmailFormat()
    {
        MockAuthentication("wagenparkbeheerder@example.com", "Wagenparkbeheerder");

        var model = new VoertuigUserModel { Email = "invalid-email-format", maand = "January", jaar = 2023 };
        var result = await _controller.GetVoertuigenPerUser(model);


        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Contains("Invalid email format.", badRequestResult.Value.ToString());
    }

    [Fact]
    public async Task GetVoertuigenPerUser_ReturnsNotFound_WhenAccountDoesNotExist()
    {
        MockAuthentication("GetVoertuigenPerUserReutnsNotFOund@example.com", "Wagenparkbeheerder");

        var model = new VoertuigUserModel { Email = "nonexistent@example.com", maand = "January", jaar = 2023 };
        var result = await _controller.GetVoertuigenPerUser(model);


        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
        Assert.Contains("Account with the provided email does not exist.", notFoundResult.Value.ToString());
    }

    [Fact]
    public async Task GetVoertuigenPerUser_ReturnsOk_WhenValidEmailAndMonthAndYear()
    {
       var accountId = Guid.NewGuid();

        var account = new Account
        {
            Id = accountId,
            Email = "getvoertuigenreturnsokmonthandyear@wagenpark.com",
            wachtwoord = "securePassword123",
            Rol = "Wagenparkbeheerder",
            BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>()
        };
        var bedrijf = new Bedrijf
        {
            Id = Guid.NewGuid(),
            Eigenaar = accountId,
            AbonnementId = 2,
            Domein = "wagenpark.com",
            BedrijfAccounts = new List<BedrijfAccounts>(),
            BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>()
        };
        var wagenparkbeheerders = new BedrijfWagenparkbeheerders
        {
            account_id = accountId,
            Account = account ?? throw new InvalidOperationException("Account not found"),
            bedrijf_id = bedrijf.Id,
            Bedrijf = bedrijf ?? throw new InvalidOperationException("Bedrijf not found")
        };

        var testaccount = new Account {
            Id = Guid.NewGuid(),
            Email = "thetestemail@wagenpark.com",
            wachtwoord = "securePassword123",
            Rol = "Zakelijke Klant",
        };

        var auto = new Voertuig
    {
        Merk = "BMW",
        Type = "X5",
        Kenteken = "AB-CD-12",
        Kleur = "Zwart",
        Aanschafjaar = "2022",
        voertuig_categorie = "SUV",
        Status = "Beschikbaar",
    };

        var verhuurAanvraag = new VerhuurAanvraag {
        AanvraagID = 999,
        Status = "geaccepteerd",
        Startdatum = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        VoertuigID = auto.VoertuigID,
        Bestemming = "Rotterdam",
        Account = testaccount,
    };
        _context.Account.Add(account);
        _context.Account.Add(testaccount);
        _context.Bedrijf.Add(bedrijf);
        _context.BedrijfWagenparkbeheerders.Add(wagenparkbeheerders);
        _context.VerhuurAanvragen.Add(verhuurAanvraag);
        _context.Voertuigen.Add(auto);
        await _context.SaveChangesAsync();

        MockAuthentication(account.Email, account.Rol);

        var model = new VoertuigUserModel { Email = testaccount.Email, maand = "January", jaar = 2023 };
        var result = await _controller.GetVoertuigenPerUser(model);
        Console.WriteLine(result);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task RemoveUserFromCompany_ReturnsOk_WhenAuthorized() {
         var accountId = Guid.NewGuid();

        var account = new Account
        {
            Id = accountId,
            Email = "removeuserfromcompany@wagenpark.com",
            wachtwoord = "securePassword123",
            Rol = "Wagenparkbeheerder",
            BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>()
        };
        var bedrijf = new Bedrijf
        {
            Id = Guid.NewGuid(),
            Eigenaar = accountId,
            AbonnementId = 2,
            Domein = "wagenpark.com",
            BedrijfAccounts = new List<BedrijfAccounts>(),
            BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>()
        };
        var wagenparkbeheerders = new BedrijfWagenparkbeheerders
        {
            account_id = accountId,
            Account = account ?? throw new InvalidOperationException("Account not found"),
            bedrijf_id = bedrijf.Id,
            Bedrijf = bedrijf ?? throw new InvalidOperationException("Bedrijf not found")
        };

        var testaccount = new Account {
            Id = Guid.NewGuid(),
            Email = "thetestemail@wagenpark.com",
            wachtwoord = "securePassword123",
            Rol = "Zakelijke Klant",
        };

        var bedrijfAccounts = new BedrijfAccounts
        {
            account_id = testaccount.Id,
            bedrijf_id = bedrijf.Id,
            Account = testaccount,
            Bedrijf = bedrijf,
        };
        _context.Account.Add(account);
        _context.Account.Add(testaccount);
        _context.Bedrijf.Add(bedrijf);
        _context.BedrijfWagenparkbeheerders.Add(wagenparkbeheerders);
        _context.BedrijfAccounts.Add(bedrijfAccounts);
        await _context.SaveChangesAsync();

        MockAuthentication(account.Email, account.Rol);

        var User = new EmailModelRemove { Email = testaccount.Email };
        var result = await _controller.RemoveUserFromCompany(User);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        }
}
}