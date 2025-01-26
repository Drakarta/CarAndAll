using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Entities;

public class VoertuigControllerTests
{
    private DbContextOptions<ApplicationDbContext> _options;
    private ApplicationDbContext _context;
    private VoertuigController _controller;

    public VoertuigControllerTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(_options);

        _controller = new VoertuigController(_context);
    }

    [Fact]
    public async Task GetVoertuigen_ReturnsOk_HasVoertuigen()
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

        await _context.SaveChangesAsync();

        var result = await _controller.GetVoertuigen();

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task CreateVoertuig_ReturnsOk_HasCreatedVoertuig()
    {
        var voertuig = new CreateVoertuigModel
        {
            Merk = "Toyota", 
            Type = "Corolla",
            Kenteken = "12-345-67",
            Kleur = "Zwart",
            Aanschafjaar = "2020",
            Status = "Beschikbaar",
            Prijs_per_dag = 76,
            Aantal_deuren = 4,
            Elektrisch = false,
            voertuig_categorie = "Auto"
        };

        var result = await _controller.CreateVoertuig(voertuig);

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task GetVoertuigByID_ReturnsOk_HasVoertuig()
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

        await _context.SaveChangesAsync();

        var result = await _controller.GetVoertuigByID(voertuig.VoertuigID);

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateVoertuig_ReturnsOk_HasUpdatedVoertuig()
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

        await _context.SaveChangesAsync();

        var voertuigUpdate = new UpdateVoertuigModel
        {
            VoertuigID = voertuig.VoertuigID,
            Merk = "Toyota1", 
            Type = "Corolla1",
            Kenteken = "12-345-67",
            Kleur = "Zwart",
            Aanschafjaar = "2020",
            Status = "Beschikbaar",
            Prijs_per_dag = 76,
            Aantal_deuren = 4,
            Elektrisch = false,
            voertuig_categorie = "Auto"
        };

        var result = await _controller.UpdateVoertuig(voertuigUpdate);

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task DeleteVoertuig_ReturnsOk_HasVerhuurAanvraag()
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

        var voertuigDelete = new DeleteVoertuigModel
        {
            VoertuigID = voertuig.VoertuigID,
        };

        var result = await _controller.DeleteVoertuig(voertuigDelete);

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task DeleteVoertuig_ReturnsOk_HasNoVerhuurAanvraag()
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

        await _context.SaveChangesAsync();

        var voertuigDelete = new DeleteVoertuigModel
        {
            VoertuigID = voertuig.VoertuigID,
        };

        var result = await _controller.DeleteVoertuig(voertuigDelete);

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
    }
}