using Microsoft.OpenApi.Models;
using Backend.Data;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "OutlookSMTP API", Version = "v1" });
        });
        services.AddTransient<EmailSencer>();
        services.AddSingleton<IConfiguration>(Configuration); // Ensure configuration is registered
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

        app.Use(async (context, next) =>
        {
            if (!context.Request.Headers.TryGetValue("Authorization", out var extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("API Key was not provided.");
                return;
            }

            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>("ApiKey");

             if (apiKey == null || !apiKey.Equals(extractedApiKey.ToString().Replace("Bearer ", "")))
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("Unauthorized client.");
                    return;
                }


            await next();
        });

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "OutlookSMTP API v1");
        });

        app.UseAuthentication();
        app.UseAuthorization();

    }
}