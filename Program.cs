// Program.cs
using Microsoft.EntityFrameworkCore;
using api_demo.Data; // THIS WAS MISSING!

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// GET CONNECTION STRING FROM RENDER
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// AUTO APPLY MIGRATIONS IN PRODUCTION
if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
