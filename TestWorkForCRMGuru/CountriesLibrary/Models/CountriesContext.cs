using CountriesLibrary.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CountriesLibrary.Models
{
    public sealed class CountriesContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Region> Regions { get; set; }

        public CountriesContext() 
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConfigurationManager.Config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
