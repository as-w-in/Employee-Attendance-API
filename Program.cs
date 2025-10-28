using Microsoft.EntityFrameworkCore;
using api_demo.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// CONNECTION STRING FROM RENDER
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// ADD THESE 2 LINES
app.UseDefaultFiles();   // <--- THIS FIXES 404
app.UseStaticFiles();    // <--- THIS SERVES CSS/JS

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// AUTO MIGRATE
if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
