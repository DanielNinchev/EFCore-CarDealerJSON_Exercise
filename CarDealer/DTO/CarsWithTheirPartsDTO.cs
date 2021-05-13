using Newtonsoft.Json;

namespace CarDealer.DTO
{
    public class CarsWithTheirPartsDTO
    {
        [JsonProperty("make")]
        public string Make { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("travelledDistance")]
        public long TravelledDistance { get; set; }

        [JsonProperty("parts")]
        public PartsOfCarsDTO[] Parts { get; set; }
    }
}
