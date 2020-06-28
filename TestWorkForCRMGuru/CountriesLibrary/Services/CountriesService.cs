using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CountriesLibrary.Managers;
using CountriesLibrary.Models;
using CountriesLibrary.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CountriesLibrary.Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository countriesRepository;
        private readonly ICrudRepository<City, int> citiesRepository;
        private readonly ICrudRepository<Region, int> regionsRepository;

        public CountriesService()
        {
            countriesRepository = new CountriesRepository();
            citiesRepository = new CrudRepository<City, int>();
            regionsRepository = new CrudRepository<Region, int>();
        }

        public async Task<CountryViewModel[]> GetCountriesAsync()
        {
            using (var context = new CountriesContext())
            {
                var countries = await countriesRepository.GetAllCountriesAsync(context).ConfigureAwait(false);
                var countriesViewModels = new List<CountryViewModel>();
                foreach (var country in countries)
                {
                    countriesViewModels.Add(MapCountryToViewModel(country));
                }

                return countriesViewModels.ToArray();
            }
        }

        public async Task<CountryViewModel> RequestCountryAsync(string countryName)
        {
            var uri = ConfigurationManager.Config.GetSection("CountriesInfoAPI")["DefaultAPI"] + countryName;
            var responseText = await WebManager.Request(uri).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<CountryViewModel[]>(responseText).FirstOrDefault();
        }

        public async Task SaveCountryAsync(CountryViewModel countryViewModel)
        {
            var capitalId = await SaveCapitalAsync(countryViewModel.Capital).ConfigureAwait(false);
            var regionId = await SaveRegionAsync(countryViewModel.Region).ConfigureAwait(false);
            using (var context = new CountriesContext())
            {
                var savedCountry = await countriesRepository.GetAll(context)
                    .FirstOrDefaultAsync(country => country.NumericCode == countryViewModel.NumericCode)
                    .ConfigureAwait(false);
                if (savedCountry == null)
                {
                    await countriesRepository
                        .AddAsync(context, MapViewModelToCountry(countryViewModel, capitalId, regionId))
                        .ConfigureAwait(false);
                }
                else
                {
                    await countriesRepository
                        .UpdateAsync(context, savedCountry.Id,
                            country => MapViewModelToCountry(countryViewModel, capitalId, regionId))
                        .ConfigureAwait(false);
                }
            }
        }

        private async Task<int> SaveCapitalAsync(string capitalName)
        {
            using (var context = new CountriesContext())
            {
                var capital = await citiesRepository.GetAll(context)
                    .FirstOrDefaultAsync(city => city.Name == capitalName)
                    .ConfigureAwait(false);
                if (capital == null)
                {
                    capital = new City() { Name = capitalName };
                    await citiesRepository.AddAsync(context, capital).ConfigureAwait(false);
                }

                await context.SaveChangesAsync().ConfigureAwait(false);
                return capital.Id;
            }
        }

        private async Task<int> SaveRegionAsync(string regionName)
        {
            using (var context = new CountriesContext())
            {
                var region = await regionsRepository.GetAll(context)
                    .FirstOrDefaultAsync(reg => reg.Name == regionName)
                    .ConfigureAwait(false);
                if (region == null)
                {
                    region = new Region() { Name = regionName };
                    await regionsRepository.AddAsync(context, region).ConfigureAwait(false);
                }

                await context.SaveChangesAsync().ConfigureAwait(false);
                return region.Id;
            }
        }

        private Country MapViewModelToCountry(CountryViewModel countryViewModel, int capitalId, int regionId)
        {
            return new Country()
            {
                Name = countryViewModel.Name,
                NumericCode = countryViewModel.NumericCode,
                CapitalId = capitalId,
                Area = float.Parse(countryViewModel.Area, CultureInfo.InvariantCulture),
                Population = countryViewModel.Population,
                RegionId = regionId
            };
        }

        private CountryViewModel MapCountryToViewModel(Country country)
        {
            return new CountryViewModel()
            {
                Name = country.Name,
                NumericCode = country.NumericCode,
                Capital = country.Capital.Name,
                Area = country.Area.ToString(CultureInfo.InvariantCulture),
                Population = country.Population,
                Region = country.Region.Name
            };
        }
    }
}
