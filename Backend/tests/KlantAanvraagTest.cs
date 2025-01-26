using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Entities;
using System.Security.Claims;

namespace Backend.test{

public class KlantAanvraagControllerTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly ApplicationDbContext _context;

    private readonly KlantAanvraagController _controller;

    public KlantAanvraagControllerTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(_options);

        _controller = new KlantAanvraagController(_context);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, "frontoffice@example.com")
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        _context.VerhuurAanvragen.RemoveRange(_context.VerhuurAanvragen);
        _context.SaveChanges();
        _context.VerhuurAanvragen.AddRange(
            new VerhuurAanvraag
            {
                AanvraagID = 1,
                Status = "geaccepteerd",
                VoertuigID = 101,
                Bestemming = "Rotterdam",
                Account = new Account
                {
                    Id = Guid.NewGuid(),
                    Email = "frontoffice@example.com",
                    wachtwoord = "hashed_password",
                    Rol = "Wagenparkbeheerder"
                }
            }
        );
    }

    [Fact]
    public async Task GetAanvragen_ReturnsOk_WhenAuthorized()
    {
        var result = await _controller.GetKlantAanvragen();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
}
}