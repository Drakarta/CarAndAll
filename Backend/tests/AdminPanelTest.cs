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
        // Test of de AddUser methode een OkObjectResult teruggeeft wanneer de gebruiker succesvol is aangemaakt
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
        // Test of de GetAllUsers methode een OkObjectResult teruggeeft met een lijst van gebruikers
public async Task GetAllUsers_ReturnsOkResult()
{

    await Task.Delay(5000);
      await _context.Database.EnsureDeletedAsync();
    await _context.Database.EnsureCreatedAsync();

    // Arrange
    var account1 = new Account
    {
        Id = Guid.NewGuid(),
        Email = "getallusersreturnsokresult001@email.exe",
        wachtwoord = "Password123",
        Rol = "User",
        Naam = "User One"
    };

    _context.Account.AddRange(account1);
    await _context.SaveChangesAsync();

    // Act
    var result = await _controller.GetAllUsers();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var returnValue = okResult.Value;
    var usersProperty = returnValue?.GetType().GetProperty("users")?.GetValue(returnValue);
    Assert.NotNull(usersProperty);
    var usersList = Assert.IsAssignableFrom<IEnumerable<object>>(usersProperty).ToList();
    Assert.Equal(1, usersList.Count); // Expecting 1 user

    dynamic user1 = usersList[0];
    Assert.Equal("getallusersreturnsokresult001@email.exe", user1.Email);
    Assert.Equal("User One", user1.Name);
    Assert.Equal("User", user1.Role);

}


        [Fact]
        // Test of de UpdateUserRole methode een OkObjectResult teruggeeft wanneer de gebruikersrol succesvol is bijgewerkt
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
        // Test of de RemoveUser methode een OkObjectResult teruggeeft wanneer de gebruiker succesvol is verwijderd
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