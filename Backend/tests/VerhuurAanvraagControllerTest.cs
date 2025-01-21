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

public class VerhuurAanvraagControllerTests
{
    private DbContextOptions<ApplicationDbContext> _options;
    private ApplicationDbContext _context;
    private VerhuurAanvraagController _controller;

    public VerhuurAanvraagControllerTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(_options);

        _controller = new VerhuurAanvraagController(_context);
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
    public async Task CreateVerhuurAanvraag_IsCreated_IsSaved()
    {
        var accountId = Guid.NewGuid();

        var account = new Account 
        {
            Id = accountId, 
            Email = "particulierehuurder@example.com", 
            wachtwoord = "securePassword123",
            Rol = "Particuliere huurder"
        };
        _context.Account.Add(account);
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

        MockAuthentication(account.Email, account.Rol);

        var verhuurAanvraagModel = new VerhuurAanvraagModel { 
                    Startdatum = DateTime.Today,
                    Einddatum = DateTime.Today.AddDays(14),
                    Bestemming = "Spanje",
                    Kilometers = 500,
                    VoertuigID = voertuig.VoertuigID,
                 };

        var result = await _controller.CreateVerhuurAanvraag(verhuurAanvraagModel);

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
    }
}