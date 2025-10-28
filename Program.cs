using Microsoft.EntityFrameworkCore;
using api_demo.Data;
using Npgsql;
using System;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// ✅ Convert Render-style DATABASE_URL to standard format
string? rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string? connectionString;

if (!string.IsNullOrEmpty(rawUrl))
{
    var databaseUri = new Uri(rawUrl);
    var userInfo = databaseUri.UserInfo.Split(':');

    var builderPg = new NpgsqlConnectionStringBuilder
    {
        Host = databaseUri.Host,
        Port = databaseUri.Port,
        Username = userInfo[0],
        Password = userInfo[1],
        Database = databaseUri.LocalPath.TrimStart('/'),
        SslMode = SslMode.Require,
        TrustServerCertificate = true
    };

    connectionString = builderPg.ToString();
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
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

if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
