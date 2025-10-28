using Microsoft.EntityFrameworkCore;
using api_demo.Data;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// ✅ 1. Try to read DATABASE_URL from Render
string? rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string? connectionString = null;

if (!string.IsNullOrEmpty(rawUrl))
{
    try
    {
        // Handle both “postgres://” and “postgresql://” URLs
        if (rawUrl.StartsWith("postgres://"))
            rawUrl = rawUrl.Replace("postgres://", "postgresql://");

        var uri = new Uri(rawUrl);
        var userInfo = uri.UserInfo.Split(':');

        var builderPg = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Username = userInfo[0],
            Password = userInfo.Length > 1 ? userInfo[1] : "",
            Database = uri.LocalPath.TrimStart('/'),
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };

        connectionString = builderPg.ConnectionString;
        Console.WriteLine("✅ Parsed connection string: " + connectionString);
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error parsing DATABASE_URL: " + ex.Message);
    }
}
else
{
    // ✅ 2. Fallback to local appsettings.json
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine("✅ Using local DefaultConnection");
}

if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("❌ Database connection string is missing! Check Render Environment Variables.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// ✅ Optional root route for testing
app.MapGet("/", () => "✅ API and Database Connected!");

if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
