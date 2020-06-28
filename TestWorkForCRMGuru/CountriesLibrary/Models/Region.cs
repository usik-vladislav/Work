using System.Collections.Generic;
using CountriesLibrary.Repositories;

namespace CountriesLibrary.Models
{
    public class Region : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Country> Countries { get; set; }

        public Region()
        {
            Countries = new List<Country>();
        }
    }
}
