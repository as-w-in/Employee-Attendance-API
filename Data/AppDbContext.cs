using Microsoft.EntityFrameworkCore;
using api_demo.Models;
namespace api_demo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Employee> Employees{ get; set; }
    }
}