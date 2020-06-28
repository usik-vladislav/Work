using CountriesLibrary.Repositories;

namespace CountriesLibrary.Models
{
    public class Country : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NumericCode { get; set; }
        public float Area { get; set; }
        public int Population { get; set; }
         
        public City Capital { get; set; }
        public int CapitalId { get; set; }

        public Region Region { get; set; }
        public int RegionId { get; set; }
    }
}
