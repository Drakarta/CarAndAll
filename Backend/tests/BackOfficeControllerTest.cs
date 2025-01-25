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

     [Fact]
    public async Task GetVerhuurAanvragen_ReturnsOk_HasVerhuurAanvragen()
    {
        var voertuig = new Voertuig 
        {
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
    
    [Fact]
    public async Task ChangeStatus_ReturnsOk_Geaccepteerd()
    {
        var voertuig = new Auto 
        {
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

        var backOfficeModel = new BackOfficeModel { AanvraagID = verhuurAanvraag1.AanvraagID, Status = "Geaccepteerd"};

        var result = await _controller.ChangeStatus(backOfficeModel);

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
    }
}