using Backend.Controllers;
using Backend.Data;
using Backend.Entities;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;

namespace Backend.Tests
{
    public class AbonnementAanvraagControllerTest
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly AbonnementAanvraagController _controller;

        public AbonnementAanvraagControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockContext = new Mock<ApplicationDbContext>(options);
            _controller = new AbonnementAanvraagController(_mockContext.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task CreateAbonnementAanvraag_ReturnsBadRequest_WhenNaamIsEmpty()
        {
            // Arrange
            var model = new AbonnementAanvraagModel { Naam = "" };

            // Act
            var result = await _controller.CreateAbonnementAanvraag(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task GetAbonnementAanvragen_ReturnsNotFound_WhenNoAanvragenFound()
        {
            // Arrange
            var mockSet = new Mock<DbSet<AbonnementAanvraag>>();
            _mockContext.Setup(c => c.AbonnementAanvragen).Returns(mockSet.Object);

            // Act
            var result = await _controller.GetAbonnementAanvragen();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task ChangeStatus_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            var model = new ChangeStatusModel { AanvraagID = 0, Status = "" };

            // Act
            var result = await _controller.ChangeStatus(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task ChangeStatus_ReturnsNotFound_WhenAanvraagNotFound()
        {
            // Arrange
            var model = new ChangeStatusModel { AanvraagID = 1, Status = "Geaccepteerd" };
            _mockContext.Setup(c => c.AbonnementAanvragen.FindAsync(model.AanvraagID))
                .ReturnsAsync((AbonnementAanvraag?)null);

            // Act
            var result = await _controller.ChangeStatus(model);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}