using System.Threading.Tasks;
using CountriesLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace CountriesLibrary.Repositories
{
    public class CountriesRepository : CrudRepository<Country, int>, ICountriesRepository
    {
        public async Task<Country> GetCountryAsync(CountriesContext context, int id)
        {
            return await context.Set<Country>()
                .Include(country => country.Capital)
                .Include(country => country.Region)
                .FirstOrDefaultAsync(country => country.Id == id);
        }

        public async Task<Country[]> GetAllCountriesAsync(CountriesContext context)
        {
            return await context.Set<Country>()
                .Include(country => country.Capital)
                .Include(country => country.Region)
                .ToArrayAsync();
        }
    }
}
