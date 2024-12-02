public class Startup
{
    // ...existing code...

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // ...existing code...

        app.UseAuthentication(); // Ensure this line is present
        app.UseAuthorization();  // Ensure this line is present

        // ...existing code...
    }
}