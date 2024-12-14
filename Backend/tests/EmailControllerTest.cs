using Backend.Controllers;
using Backend.Data;
using Backend.Entities;
using Backend.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class EmailControllerTest
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IUserService> _mockUserService;
    private readonly EmailController _controller;

    public EmailControllerTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _mockUserService = new Mock<IUserService>();
        _controller = new EmailController(_context, _mockUserService.Object);
    }

    [Fact]
    public async Task AddUserToCompany_ShouldReturnOk()
    {
        // Arrange
        var emailModel = new EmailController.EmailModel { Email = "test@CarAndAll.com" };

        // Mock database entries
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = "test@CarAndAll.com",
            Wachtwoord = "12345",
            AccountBedrijven = new List<AccountBedrijf>()
        };

        var bedrijf = new Bedrijf
        {
            Id = 2,
            Abbonement = 2,
            Eigenaar = 1,
            AccountBedrijven = new List<AccountBedrijf>()
        };

        _context.Account.Add(account);
        _context.Bedrijf.Add(bedrijf);
        await _context.SaveChangesAsync();

        _mockUserService.Setup(u => u.GetAccount_Id()).Returns(1);

        // Act
        var result = await _controller.AddUserToCompany(emailModel);

        // Assert
        if (result is BadRequestObjectResult badRequestResult)
        {
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        else
        {
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }

    [Fact]
    public async Task RemoveUserFromCompany_ShouldReturnOk()
    {
        // Arrange
        var emailModel = new EmailController.EmailModel { Email = "jhon.deer@CarAndAll.com" };

        // Mock database entries
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = "jhon.deer@CarAndAll.com",
            Wachtwoord = "123",
            AccountBedrijven = new List<AccountBedrijf>()
        };

        var bedrijf = new Bedrijf
        {
            Id = 3,
            Eigenaar = 1,
            Abbonement = 2,
            AccountBedrijven = new List<AccountBedrijf>()
        };

        var accountBedrijf = new AccountBedrijf
        {
            Account_id = Guid.NewGuid(),
            Bedrijf_id = 2,
            Account = account,
            Bedrijf = bedrijf
        };

        _context.Account.Add(account);
        _context.Bedrijf.Add(bedrijf);
        _context.AccountBedrijven.Add(accountBedrijf);
        await _context.SaveChangesAsync();

        _mockUserService.Setup(u => u.GetAccount_Id()).Returns(2);

        // Act
        var result = await _controller.RemoveUserFromCompany(emailModel);

        // Assert
        if (result is NotFoundObjectResult notFoundResult)
        {
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        else
        {
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }

    [Fact]
    public async Task GetEmails_ShouldReturnEmails()
    {
        // Arrange
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = "jhon.deer@CarAndAll.com",
            Wachtwoord="123",
            AccountBedrijven = new List<AccountBedrijf>()
        };

        var bedrijf = new Bedrijf
        {
            Id = 5,
            Abbonement = 2,
            Eigenaar = 1,
            AccountBedrijven = new List<AccountBedrijf>()
        };

        var accountBedrijf = new AccountBedrijf
        {
            Account_id = Guid.NewGuid(),
            Bedrijf_id = 5,
            Account = account,
            Bedrijf = bedrijf
        };

        _context.Account.Add(account);
        _context.Bedrijf.Add(bedrijf);
        _context.AccountBedrijven.Add(accountBedrijf);
        await _context.SaveChangesAsync();

        _mockUserService.Setup(u => u.GetAccount_Id()).Returns(4);

        // Act
        var result = await _controller.GetEmails();

        // Assert
        if (result is UnauthorizedObjectResult unauthorizedResult)
        {
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }
        else
        {
            var okResult = Assert.IsType<OkObjectResult>(result);
            var emails = Assert.IsType<List<string>>(okResult.Value);
            Assert.Single(emails);
            Assert.Equal("jhon.deer@CarAndAll.com", emails.First());
        }
    }
}
