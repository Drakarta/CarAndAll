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
        private readonly AbonnementAanvraagController _controller;

        public AbonnementAanvraagControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var context = new ApplicationDbContext(options);
            _controller = new AbonnementAanvraagController(context);

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
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var controller = new AbonnementAanvraagController(context);

                // Act
                var result = await controller.GetAbonnementAanvragen();

                // Assert
                var actionResult = Assert.IsType<ActionResult<IEnumerable<AbonnementAanvraag>>>(result);
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
                Assert.Equal(404, notFoundResult.StatusCode);
            }
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
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var controller = new AbonnementAanvraagController(context);
                var model = new ChangeStatusModel { AanvraagID = 1, Status = "Geaccepteerd" };

                // Act
                var result = await controller.ChangeStatus(model);

                // Assert
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal(404, notFoundResult.StatusCode);
            }
        }
    }
}