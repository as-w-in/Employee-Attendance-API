// Program.cs
using Microsoft.EntityFrameworkCore;
using api_demo.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// GET CONNECTION STRING FROM RENDER (PRIORITY) OR LOCAL
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("Database connection string is missing!");

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// SERVE FRONTEND (wwwroot)
app.UseDefaultFiles();   // Serves index.html
app.UseStaticFiles();    // Serves CSS, JS, images

// Middleware
app.UseHttpsRedirection();
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
