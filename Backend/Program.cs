using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Configure services, hier moet je je .cs file waar die verbinden maakt met db zetten
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddHttpContextAccessor(); // Ensure this line is present

builder.Services.AddControllers();

// CORS, Swagger, etc.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure middleware
app.UseCors("AllowAllOrigins");
app.UseSwagger(); // Ensure this line is present
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CarAndAll API V1"));

// Ensure authentication and authorization middleware are present
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();