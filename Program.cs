using Microsoft.EntityFrameworkCore;
using api_demo.Data;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Read connection string (Render env var)
var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(dbUrl))
    throw new InvalidOperationException("DATABASE_URL environment variable is missing!");

// Convert the URL-style connection string to Npgsql format
var databaseUri = new Uri(dbUrl);
var userInfo = databaseUri.UserInfo.Split(':');
var username = userInfo[0];
var password = userInfo.Length > 1 ? userInfo[1] : "";
var host = databaseUri.Host;
var port = databaseUri.Port > 0 ? databaseUri.Port : 5432;
var database = databaseUri.LocalPath.TrimStart('/');

// Build a proper Npgsql connection string
var npgsqlBuilder = new NpgsqlConnectionStringBuilder
{
    Host = host,
    Port = port,
    Username = username,
    Password = password,
    Database = database,
    SslMode = SslMode.Require,
    TrustServerCertificate = true
};

var connectionString = npgsqlBuilder.ConnectionString;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
