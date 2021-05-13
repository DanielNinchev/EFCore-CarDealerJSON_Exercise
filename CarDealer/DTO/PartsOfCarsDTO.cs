using Newtonsoft.Json;

namespace CarDealer.DTO
{
    public class PartsOfCarsDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}
