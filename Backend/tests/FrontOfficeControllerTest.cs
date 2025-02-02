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
        _controller = new FrontOfficeController(_context);
        InitializeDatabase();
    
    }
    // Initializeren van de database en ook geseed
    private void InitializeDatabase()
    {
        _context.VerhuurAanvragen.RemoveRange(_context.VerhuurAanvragen);
        _context.SaveChanges();
        var aanvraag = new VerhuurAanvraag
        {
            AanvraagID = 1322,
            Status = "geaccepteerd",
            VoertuigID = 105,
            Bestemming = "Rotterdam",
        };
        _context.VerhuurAanvragen.Add(aanvraag);
        _context.SaveChanges();
    }

 
    //testen of de methode ChangeStatus werkt, en returns not found teruggeeft als de aanvraag niet bestaat
    [Fact]
    public async Task ChangeStatus_ReturnsNotFound_WhenRequestDoesNotExist()
    {

        // Arrange
        var model = new Request { AanvraagID = 998, NewStatus = "ingenomen", SchadeInfo = "" };

        // Act
        var result = await _controller.ChangeStatus(model);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Aanvraag not found", notFoundResult.Value);
    }

    //testen of de methode ChangeStatus werkt, en returns bad request teruggeeft als de status niet geldig is
    [Fact]
    public async Task ChangeStatus_ReturnsBadRequest_WhenStatusIsInvalid()
    {

        // Arrange
        var model = new Request { AanvraagID = 1322, NewStatus = "invalid", SchadeInfo = "" };

        // Act
        var result = await _controller.ChangeStatus(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid status", badRequestResult.Value);
    }

    //testen of de methode GetVerhuurAanvragenWithStatus werkt, en returns de geaccepteerde aanvragen terug
   [Fact]
    public async Task GetVerhuurAanvragenWithStatus_ReturnsAcceptedRequests()
    {
        // Arrange
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

        Assert.Equal(1322, aanvraagID);
        Assert.Equal("geaccepteerd", status);
    }


}
}