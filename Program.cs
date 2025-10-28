using Microsoft.EntityFrameworkCore;
using api_demo.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers(); // Enables Controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add PostgreSQL connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
// Map controllers
app.MapControllers();
app.UseStaticFiles();
app.Run();
