using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Controllers;
using Backend.Data;
using Backend.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;

namespace Backend.Tests
{
    public class AbonnementControllerTest
    {
        private readonly ApplicationDbContext _context;
        private readonly AbonnementController _controller;

        public AbonnementControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new AbonnementController(_context);

            var data = new List<Abonnement>().AsQueryable();

            var mockSet = new Mock<DbSet<Abonnement>>();
            mockSet.As<IQueryable<Abonnement>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Abonnement>(data.Provider));
            mockSet.As<IQueryable<Abonnement>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Abonnement>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Abonnement>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mockSet.As<IAsyncEnumerable<Abonnement>>().Setup(m => m.GetAsyncEnumerator(default)).Returns(new TestAsyncEnumerator<Abonnement>(data.GetEnumerator()));

            _context.Abonnement = mockSet.Object;
        }

        

        [Fact]
        public async Task GetAbonnementById_ReturnsNotFound_WhenAbonnementDoesNotExist()
        {
            // Act
            var result = await _controller.GetAbonnementById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Abonnement not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetAbonnementById_ReturnsOkResult_WithAbonnement()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var controller = new AbonnementController(context);
                var abonnement = new Abonnement { Id = 1, Naam = "Abonnement 1" };
                context.Abonnement.Add(abonnement);
                await context.SaveChangesAsync();

                // Act
                var result = await controller.GetAbonnementById(1);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnValue = Assert.IsType<Abonnement>(okResult.Value);
                Assert.Equal(1, returnValue.Id);

            }
        }
        

        [Fact]
        public async Task CreateAbonnement_ReturnsCreatedAtActionResult_WithNewAbonnement()
        {
            // Arrange
            var newAbonnement = new Abonnement { Id = 1, Naam = "New Abonnement" };

            // Act
            var result = await _controller.CreateAbonnement(newAbonnement);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Abonnement>(createdAtActionResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task GetAbonnementen_ReturnsOkResult_WithListOfAbonnementen()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                // Clear the database
                context.Abonnement.RemoveRange(await context.Abonnement.ToListAsync());
                await context.SaveChangesAsync();

                var controller = new AbonnementController(context);
                var abonnementen = new Abonnement { Id = 65, Naam = "Abonnement 2" };
                context.Abonnement.AddRange(abonnementen);
                await context.SaveChangesAsync();

                // Act
                var result = await controller.GetAbonnementen();
                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnValue = Assert.IsType<List<Abonnement>>(okResult.Value);
                Assert.Equal(1, returnValue.Count);
            }
        }
    }
}