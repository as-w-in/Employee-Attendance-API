using Microsoft.EntityFrameworkCore;
using api_demo.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- DATABASE CONNECTION HANDLER ---
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrEmpty(databaseUrl))
{
    postgresql://employee_attendance_db_user:Ww9zIBw68fhIYu78LodwCkJ5MxBZpGZ8@dpg-d40977s9c44c738ntomg-a.oregon-postgres.render.com/employee_attendance_db
    connectionString = databaseUrl;
}
else
{
    // Fallback for local development (uses appsettings.json)
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- BUILD APP ---
var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();

app.Run();
