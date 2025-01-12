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
        var bedrijfId = Guid.NewGuid();

        var account = new Account 
        {
            Id = accountId, 
            Email = "wagenparkbeheerder@example.com", 
            wachtwoord = "hashed_password",
            Rol = "Wagenparkbeheerder"
        };
        var bedrijf = new Bedrijf 
        {   
            Id = bedrijfId,
            Eigenaar = accountId, 
            Abbonement = "kleinste", 
            Domein = "exmample.com",
            BedrijfAccounts = new List<BedrijfAccounts>() 
        };

        _context.Account.Add(account);
        _context.Bedrijf.Add(bedrijf);
        await _context.SaveChangesAsync();

        // Mock Authentication to simulate authorized user
        MockAuthentication(account.Email, account.Rol);

        // Act: Calling the controller method
        var result = await _controller.GetEmails();
        Console.WriteLine($"Actual result: {result}");

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
    public async Task AddUserToCompany_ReturnsOk_WhenAuthorized()
    {
        var accountId = Guid.NewGuid();
        var bedrijfId = Guid.NewGuid();

        var account = new Account 
        {
            Id = accountId, 
            Email = "wagenparkbeheerder@example.com", 
            wachtwoord = "securePassword123" ,
            Rol = "Wagenparkbeheerder"
        };
        var bedrijf = new Bedrijf 
        {  
            Id = bedrijfId,
            Eigenaar = accountId, 
            Abbonement = "kleinste", 
            Domein = "exmample.com",
            BedrijfAccounts = new List<BedrijfAccounts>() 
        };

        _context.Account.Add(account);
        _context.Bedrijf.Add(bedrijf);
        await _context.SaveChangesAsync();

        // Mock Authentication to simulate authorized user
        MockAuthentication(account.Email, account.Rol);

        // Act: Calling the controller method
        var emailModel = new EmailModel { Email = "test@example.com" };
        var result = await _controller.AddUserToCompany(emailModel);
        Console.WriteLine($"Actual result: {result}");

        // Assert: Ensure the result is Ok

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task AddUserToCompany_ReturnsBadRequest_WhenEmailFormatIsInvalid()
    {
        // Mock Authentication to simulate authorized user
        var accountId = Guid.NewGuid();
        var bedrijfId = Guid.NewGuid();

        var account = new Account 
        {
            Id = accountId, 
            Email = "wagenparkbeheerder@example.com", 
            wachtwoord = "securePassword123" ,
            Rol = "Wagenparkbeheerder"
            
        };
        var bedrijf = new Bedrijf 
        {  
            Id = bedrijfId,
            Eigenaar = accountId, 
            Abbonement = "kleinste", 
            Domein = "exmample.com",
            BedrijfAccounts = new List<BedrijfAccounts>() 
        };

        _context.Account.Add(account);
        _context.Bedrijf.Add(bedrijf);
        await _context.SaveChangesAsync();

        // Mock Authentication to simulate authorized user
        MockAuthentication(account.Email, account.Rol);

        // Act: Calling the controller method with invalid email format
        var emailModel = new EmailModel { Email = "invalid-email-format" };
        var result = await _controller.AddUserToCompany(emailModel);
        Console.WriteLine($"Actual result: {result}");

        // Assert: Ensure the result is BadRequest
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Contains("FalseFormatEmail", badRequestResult.Value.ToString());
    }

    [Fact]
    public async Task AddUserToCompany_ReturnsBadRequest_WhenUserAddsSelf()
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
            Abbonement = "kleinste", 
            Domein = "exmample.com",
            BedrijfAccounts = new List<BedrijfAccounts>() 

        };

        _context.Account.Add(account);
        _context.Bedrijf.Add(bedrijf);
        await _context.SaveChangesAsync();

        
        MockAuthentication(account.Email, account.Rol);

        // Act
        var emailModel = new EmailModel { Email = "wagenparkbeheerder@example.com" };
        var result = await _controller.AddUserToCompany(emailModel);
        Console.WriteLine($"Actual result: {result}");

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Contains("User cannot add himself/herself to the company.", badRequestResult.Value.ToString());
    }


    

    [Fact]
    public async Task AddUserToCompany_ReturnsBadRequest_WhenDomainIsInvalid()
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
            Abbonement = "kleinste", 
            Domein = "example.com", // Set a specific domain
            BedrijfAccounts = new List<BedrijfAccounts>() 
        };

        _context.Account.Add(account);
        _context.Bedrijf.Add(bedrijf);
        await _context.SaveChangesAsync();

        // Mock Authentication to simulate authorized user
        MockAuthentication(account.Email, account.Rol);

        // Act: Calling the controller method with an email that does not match the domain
        var emailModel = new EmailModel { Email = "test@invalid.com" };
        var result = await _controller.AddUserToCompany(emailModel);
        Console.WriteLine($"Actual result: {result}");

        // Assert: Ensure the result is BadRequest
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Contains("FalseDomein", badRequestResult.Value.ToString());
    }

}
