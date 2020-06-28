using CountriesLibrary.Repositories;

namespace CountriesLibrary.Models
{
    public class City : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Country Country { get; set; }
    }
}
