using System.Threading.Tasks;
using CountriesLibrary.Models;

namespace CountriesLibrary.Repositories
{
    public interface ICountriesRepository : ICrudRepository<Country, int>
    {
        Task<Country> GetCountryAsync(CountriesContext context, int id);
        Task<Country[]> GetAllCountriesAsync(CountriesContext context);
    }
}
