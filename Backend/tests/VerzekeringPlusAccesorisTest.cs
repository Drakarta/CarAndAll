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
using Microsoft.VisualBasic;
using System.Threading.Tasks;

namespace Backend.Tests {
public class VerzekeringPlusAccesorisTest
{    
    private DbContextOptions<ApplicationDbContext> _options;
    private ApplicationDbContext _context;
    private readonly VerhuurAanvraagController _controller;

    public VerzekeringPlusAccesorisTest()
    {
                _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(_options);
        _controller = new VerhuurAanvraagController(_context);
    }
    //zorgt ervoor dat de databse leeg is voor elke test
    private async Task ClearDatabase()
    {
        _context.VerhuurAanvragen.RemoveRange(_context.VerhuurAanvragen);
        _context.Accessoires.RemoveRange(_context.Accessoires);
        _context.Voertuigen.RemoveRange(_context.Voertuigen);
        _context.Account.RemoveRange(_context.Account);
        await _context.SaveChangesAsync();


    }
    //een "nep" authenticatie voor de test
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

        //Testen of de verhuuraanvraag aangemaakt wordt als de verzekering multiplier 0 is
    [Fact]
    public async Task CreateVerhuurAanvraag_ShouldReturnBadRequest_WhenVerzekeringMultiplierIsZero()
    {

         // Arrange
                 await ClearDatabase();
   var account = new Account
    {
            Id = Guid.NewGuid(),
            Email = "Badaccesoires@example.com",
            wachtwoord = "hashed_password",
            Rol = "Particuliere huurder",
            BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>(),
        
    };

        await _context.Account.AddAsync(account);
        MockAuthentication(account.Email, account.Rol);

        
        var model = new VerhuurAanvraagModel { Bestemming = "Amsterdam", Kilometers = 100, Verzekering_multiplier = 0, VoertuigID  = 1, Startdatum = DateTime.Now, Einddatum = DateTime.Now.AddDays(1)  };   

        // Act
        var result = await _controller.CreateVerhuurAanvraag(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    //Testen of de verhuuraanvraaag aangemaakt wordt als alles klopt
    [Fact]
    public async Task CreateVerhuurAanvraag_ShouldReturnOk_WhenModelIsValid()
    {
        await Task.Delay(6000);
         // Arrange
         await ClearDatabase();
         string guidString = "12345678-1234-1234-1234-1234567890ab";
Guid guid = Guid.Parse(guidString);
             var account = new Account
             {
            Id = guid,
            Email = "Badaccesoires@example.com",
            wachtwoord = "hashed_password",
            Rol = "Particuliere huurder",
            BedrijfWagenparkbeheerders = new List<BedrijfWagenparkbeheerders>(),
            };
        _context.Account.Add(account);
        await _context.SaveChangesAsync();

        MockAuthentication(account.Email, account.Rol);

        var voertuig = new Auto 
        {
            VoertuigID = 273745434,
            Merk = "Toyota", 
            Type = "Corolla",
            Kenteken = "12-345-67",
            Kleur = "Zwart",
            Aanschafjaar = "2020",
            Status = "Beschikbaar",
            Prijs_per_dag = 76,
            Aantal_deuren = 4,
            Elektrisch = false
        };
        await _context.Voertuigen.AddAsync(voertuig);

        var model = new VerhuurAanvraagModel
        { 
            
            Bestemming = "Amsterdam",
            Kilometers = 100,
            Verzekering_multiplier = 1.0,
            VoertuigID = 273745434,
            Startdatum = DateTime.Now,
            Einddatum = DateTime.Now.AddDays(1),
            Accessoires = new List<AccessoireList>
            {
                new AccessoireList { AccessoireNaam = "GPS", Aantal = 1 }
            }
        };

        var accessoire = new Accessoire { AccessoireNaam = "GPS", Max_aantal = 5, Extra_prijs_per_dag = 10, VerhuurAanvraagAccessoires = new List<VerhuurAanvraagAccessoire>() };

       await  _context.Accessoires.AddAsync(accessoire);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.CreateVerhuurAanvraag(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
}
}