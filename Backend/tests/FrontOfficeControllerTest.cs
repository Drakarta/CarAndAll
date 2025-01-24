using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Controllers;
using Backend.Data;
using Backend.Entities;

namespace Backend.test{

public class FrontOfficeControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly FrontOfficeController _controller;

    public FrontOfficeControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);

        _context.VerhuurAanvragen.RemoveRange(_context.VerhuurAanvragen);
        _context.SaveChanges();

        _context.VerhuurAanvragen.AddRange(
            new VerhuurAanvraag { AanvraagID = 1, Status = "geaccepteerd", VoertuigID = 101, Bestemming = "Rotterdam" },
            new VerhuurAanvraag { AanvraagID = 2, Status = "geweigerd", VoertuigID = 102, Bestemming = "Amsterdam" }
        );

        _context.SaveChanges();

        _controller = new FrontOfficeController(_context);
    }

    [Fact]
    public async Task ChangeStatus_ReturnsNotFound_WhenRequestDoesNotExist()
    {
        // Arrange
        var model = new Request { AanvraagID = 999, NewStatus = "ingenomen", SchadeInfo = "" };

        // Act
        var result = await _controller.ChangeStatus(model);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Aanvraag not found", notFoundResult.Value);
    }

    [Fact]
    public async Task ChangeStatus_ReturnsBadRequest_WhenStatusIsInvalid()
    {
        // Arrange
        var model = new Request { AanvraagID = 1, NewStatus = "invalid", SchadeInfo = "" };

        // Act
        var result = await _controller.ChangeStatus(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid status", badRequestResult.Value);
    }

    [Fact]
    public async Task GetVerhuurAanvragenWithStatus_ReturnsAcceptedRequests()
    {
        // Act
        var result = await _controller.GetVerhuurAanvragenWithStatus();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var requests = okResult.Value as IEnumerable<object>;
        Assert.NotNull(requests);
        Assert.Single(requests);


        var request = requests.First();
        var type = request.GetType();

        var aanvraagID = type.GetProperty("AanvraagID")?.GetValue(request, null);
        var status = type.GetProperty("Status")?.GetValue(request, null);


        Assert.Equal(1, aanvraagID);
        Assert.Equal("geaccepteerd", status);
    }

}
}