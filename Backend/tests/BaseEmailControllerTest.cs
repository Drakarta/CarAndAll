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

public class WagenparkbeheerderEmailControllerTests
{
    private DbContextOptions<ApplicationDbContext> _options;
    private ApplicationDbContext _context;
    private Mock<IEmailSender> _mockEmailSender;
    private WagenparkbeheerderEmailController _controller;

    public WagenparkbeheerderEmailControllerTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(_options);

        _mockEmailSender = new Mock<IEmailSender>();
        _controller = new WagenparkbeheerderEmailController(_context, _mockEmailSender.Object);
    }

    [Fact]
    public async Task GetEmails_ReturnsOk_WhenAuthorized()
    {
        var accountId = Guid.NewGuid();

        var account = new Account 
        {
            Id = accountId, 
            Email = "wagenparkbeheerder@example.com", 
            wachtwoord = "securePassword123" 
        };
        var bedrijf = new Bedrijf 
        { 
            Eigenaar = accountId, 
            Abbonement = "Standard", 
            BedrijfAccounts = new List<BedrijfAccounts>() 
        };

        _context.Account.Add(account);
        _context.Bedrijf.Add(bedrijf);
        await _context.SaveChangesAsync();

        // Mock Authentication to simulate authorized user
        MockAuthentication("wagenparkbeheerder@example.com");

        // Act: Calling the controller method
        var result = await _controller.GetEmails();

        // Assert: Ensure the result is Ok
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    private void MockAuthentication(string email)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, "Wagenparkbeheerder")
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
}
