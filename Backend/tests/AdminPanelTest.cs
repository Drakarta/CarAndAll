using Backend.Controllers;
using Backend.Data;
using Backend.Entities;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Backend.test
{
    public class AdminPanelTest
    {
        private readonly ApplicationDbContext _context;
        private readonly AccountController _controller;

        public AdminPanelTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new AccountController(_context, true);
        }

        [Fact]
public async Task GetAllUsers_ReturnsOkResult()
{
    // Arrange
    var users = new List<Account>
    {
        new Account { Id = Guid.NewGuid(), Email = "user1@example.com", Rol = "User", Naam = "User One", wachtwoord = "defaultPassword" },
        new Account { Id = Guid.NewGuid(), Email = "user2@example.com", Rol = "Admin", Naam = "User Two", wachtwoord = "defaultPassword" }
    };

    _context.Account.AddRange(users);
    await _context.SaveChangesAsync();

    // Act
    var result = await _controller.GetAllUsers();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var returnValue = okResult.Value;
    var usersProperty = returnValue?.GetType().GetProperty("users")?.GetValue(returnValue);
    Assert.NotNull(usersProperty);
    var usersList = Assert.IsAssignableFrom<IEnumerable<object>>(usersProperty).ToList();
    Assert.Equal(2, usersList.Count);

    dynamic user1 = usersList[0];
    Assert.Equal("user1@example.com", user1.Email);
    Assert.Equal("User One", user1.Name);
    Assert.Equal("User", user1.Role);

    dynamic user2 = usersList[1];
    Assert.Equal("user2@example.com", user2.Email);
    Assert.Equal("User Two", user2.Name);
    Assert.Equal("Admin", user2.Role);
}

        [Fact]
public async Task AddUser_ReturnsOkResult()
{
    // Arrange
    var model = new LoginRegisterModel
    {
        Email = "newuser@example.com",
        Password = "Password123",
        Role = "User",
        Naam = "New User",
        Address = "123 Main St",
        PhoneNumber = "123-456-7890"
    };

    // Act
    var result = await _controller.AddUser(model);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var returnValue = okResult.Value;
    Assert.Equal("User created successfully.", returnValue.GetType().GetProperty("Message").GetValue(returnValue));
}

        [Fact]
public async Task UpdateUserRole_ReturnsOkResult()
{
    // Arrange
    var userId = Guid.NewGuid();
    var user = new Account { Id = userId, Email = "user@example.com", Rol = "User", Naam = "User Name", wachtwoord = "defaultPassword" };

    _context.Account.Add(user);
    await _context.SaveChangesAsync();

    // Act
    var updateUserRoleModel = new UpdateUserRoleModel { Role = "Admin", Email = "user@example.com", Naam = "Updated User" };
    var result = await _controller.UpdateUserRole(userId, updateUserRoleModel);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var returnValue = okResult.Value;
    Assert.Equal("User role updated successfully.", returnValue.GetType().GetProperty("message").GetValue(returnValue));
}

        [Fact]
public async Task RemoveUser_ReturnsOkResult()
{
    // Arrange
    var userId = Guid.NewGuid();
    var user = new Account { Id = userId, Email = "user@example.com", Rol = "User", Naam = "User Name", wachtwoord = "defaultPassword" };

    _context.Account.Add(user);
    await _context.SaveChangesAsync();

    // Act
    var result = await _controller.RemoveUser(userId);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var returnValue = okResult.Value;
    Assert.NotNull(returnValue);
    var messageProperty = returnValue.GetType().GetProperty("message");
    Assert.NotNull(messageProperty);
    var messageValue = messageProperty.GetValue(returnValue);
    Assert.Equal("User removed successfully.", messageValue);
}
    }
}