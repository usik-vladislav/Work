using Newtonsoft.Json;

namespace CountriesLibrary.Models
{
    public class CountryViewModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("numericCode")]
        public string NumericCode { get; set; }
        [JsonProperty("capital")]
        public string Capital { get; set; }
        [JsonProperty("area")]
        public string Area { get; set; }
        [JsonProperty("population")]
        public int Population { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
    }
}
