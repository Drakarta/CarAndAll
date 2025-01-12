using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Backend.Entities;
using Backend.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;

public class KlantAanvraagControllerTests
{
    private DbContextOptions<ApplicationDbContext> _options;
    private ApplicationDbContext _context;

    private KlantAanvraagController _controller;

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
            new VerhuurAanvraag { AanvraagID = 1, Status = "geaccepteerd", VoertuigID = 101, Bestemming = "Rotterdam", account = new Account{Id = Guid.NewGuid(),
                Email = "frontoffice@example.com",
                wachtwoord = "hashed_password",
                Rol = "Wagenparkbeheerder"}}
        );
    }

    [Fact]
    public async Task GetAanvragen_ReturnsOk_WhenAuthorized()
    {
        var result = await _controller.GetKlantAanvragen();

        // Assert: Ensure the result is Ok
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
}