using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Backend.Entities;

public class PrivacyControllerTest
{
    private readonly PrivacyController _controller;
    private readonly ApplicationDbContext _context;

    public PrivacyControllerTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new PrivacyController(_context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
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
    public async Task GetTextTest()
    {

        var result = await _controller.GetPrivacy("Privacy");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateTextTest()
    {
        var newPrivacy = new Text
        {
            Type = "Privacy",
            Content = "This is the privacy text"
        };
        _context.Texts.Add(newPrivacy);
        await _context.SaveChangesAsync();

        var newText = new TextModelUpdate
        {
            Type = "Privacy",
            Content = "This is the updated privacy text"
        };

        var result = await _controller.PostPrivacy(newText);

        // Assert: Ensure the result is Ok
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
}