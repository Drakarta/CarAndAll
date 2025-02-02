using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Entities;

public class BackOfficeControllerTests
{
    private DbContextOptions<ApplicationDbContext> _options;
    private ApplicationDbContext _context;
    private BackOfficeController _controller;

    public BackOfficeControllerTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(_options);

        _controller = new BackOfficeController(_context);


    }

    //Testen of het ophalen van verhuuraanvragen Ok returned
    [Fact]
    public async Task GetVerhuurAanvragen_ReturnsOk_HasVerhuurAanvragen()
    {

        var voertuig = new Voertuig
        {
            VoertuigID = 2,
            Merk = "Toyota",
            Type = "Corolla",
            Kenteken = "12-345-67",
            Kleur = "Zwart",
            Aanschafjaar = "2020",
            Status = "Beschikbaar",
            Prijs_per_dag = 76,
            // Categorie = "Auto"
        };
        _context.Voertuigen.Add(voertuig);

        var accountId = Guid.NewGuid();

        var account = new Account
        {
            Id = accountId,
            Email = "particulierehuurder@example.com",
            wachtwoord = "securePassword123",
            Rol = "Particuliere huurder"
        };
        _context.Account.Add(account);

        var verhuurAanvraag1 = new VerhuurAanvraag
        {  
            AanvraagID = 2,
            Startdatum = DateTime.Today,
            Einddatum = DateTime.Today.AddDays(14),
            Bestemming = "Spanje",
            Kilometers = 500,
            VoertuigID = voertuig.VoertuigID,
            Voertuig = voertuig,
            Account = account,
            Status = "In behandeling"
        };

        _context.VerhuurAanvragen.Add(verhuurAanvraag1);
        await _context.SaveChangesAsync();

        var result = await _controller.GetVerhuurAanvragen();

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);

        
    }

    //Testen of de changestatus Ok returned voor geaccepteerd
    [Fact]
    public async Task ChangeStatus_ReturnsOk_Geaccepteerd()
    {
    
       var voertuig = new Voertuig
        {
            VoertuigID = 24538534,
            Merk = "Toyota",
            Type = "Corolla",
            Kenteken = "12-345-67",
            Kleur = "Zwart",
            Prijs_per_dag = 76,
            Aanschafjaar = "2020",
            Status = "Beschikbaar",
        };
   

        var accountId = Guid.NewGuid();

        var account = new Account
        {
            Id = accountId,
            Email = "particulierehuurder456@example.com",
            wachtwoord = "securePassword123",
            Rol = "Particuliere huurder"
        };
        _context.Account.Add(account);

        var verhuurAanvraag1 = new VerhuurAanvraag
        {  
            AanvraagID = 2355,
            Startdatum = DateTime.Today,
            Einddatum = DateTime.Today.AddDays(14),
            Bestemming = "Spanje",
            Kilometers = 500,
            Voertuig = voertuig,
            Account = account,
            Status = "In behandeling"
        };

        _context.VerhuurAanvragen.Add(verhuurAanvraag1);
        await _context.SaveChangesAsync();

        var backOfficeModel = new BackOfficeModel { AanvraagID = 2355, Status = "Geaccepteerd" };

        var result = await _controller.ChangeStatus(backOfficeModel);
    

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
    }

    //Testen of de changestatus Bad Request returned voor geaccepteerd niet valid
    [Fact]
    public async Task ChangeStatus_ReturnsBadRequest_Geaccepteerd_NoValidVerhuuraanvraag()
    {
    
       var voertuig = new Voertuig
        {
            Merk = "Toyota",
            Type = "Corolla",
            Kenteken = "12-345-67",
            Kleur = "Zwart",
            Prijs_per_dag = 76,
            Aanschafjaar = "2020",
            Status = "Beschikbaar",
        };
   

        var accountId = Guid.NewGuid();

        var account = new Account
        {
            Id = accountId,
            Email = "particulierehuurder@example.com",
            wachtwoord = "securePassword123",
            Rol = "Particuliere huurder"
        };
        _context.Account.Add(account);

        var verhuurAanvraag1 = new VerhuurAanvraag
        {  
            AanvraagID = 24000,
            Startdatum = DateTime.Today,
            Einddatum = DateTime.Today.AddDays(14),
            Bestemming = "Spanje",
            Kilometers = 500,
            Voertuig = voertuig,
            Account = account,
            Status = "In behandeling"
        };

        _context.VerhuurAanvragen.Add(verhuurAanvraag1);
        await _context.SaveChangesAsync();

        var backOfficeModel = new BackOfficeModel { AanvraagID = 999, Status = "Geaccepteerd" };

        var result = await _controller.ChangeStatus(backOfficeModel);
    

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        Assert.Equal(400, badRequestResult.StatusCode);
    }
}