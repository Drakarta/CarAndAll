using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Microsoft.AspNetCore.Identity;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddScoped<IEmailSender, EmailSencer>();
builder.Services.AddScoped<EmailSencer>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/Account/login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin");
});
    options.AddPolicy("Wagenparkbeheerder", policy =>
    {
    policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
    policy.RequireAuthenticatedUser();
    policy.RequireRole("Wagenparkbeheerder", "Admin");
    });
    options.AddPolicy("Zakelijkeklant", policy =>
    {
    policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
    policy.RequireAuthenticatedUser();
    policy.RequireRole("Zakelijkeklant", "Admin");
    });
    options.AddPolicy("Particuliere huurder", policy =>
    {
        policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Particuliere huurder", "Admin");
    });
     options.AddPolicy("FrontOffice", policy =>
    {
    policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
    policy.RequireAuthenticatedUser();
    policy.RequireRole("Frontofficemedewerker", "Admin");
    });
});


Console.WriteLine("Authorization policies configured: AdminPolicy, Wagenparkbeheerder, Zakelijkeklant");

var app = builder.Build();

app.UseCors("AllowSpecificOrigins");

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CarAndAll API V1"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
