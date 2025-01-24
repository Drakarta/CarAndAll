using Backend.Controllers;
using Backend.Data;
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
            Categorie = "Auto"
        };
        _context.Voertuigen.Add(voertuig);

        await _context.SaveChangesAsync();

        var result = await _controller.GetVoertuigen();

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
    }
}