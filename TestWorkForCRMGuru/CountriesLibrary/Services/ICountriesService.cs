using System.Threading.Tasks;
using CountriesLibrary.Models;

namespace CountriesLibrary.Services
{
    public interface ICountriesService
    {
        Task<CountryViewModel[]> GetCountriesAsync();
        Task<CountryViewModel> RequestCountryAsync(string countryName);
        Task SaveCountryAsync(CountryViewModel countryViewModel);
    }
}
