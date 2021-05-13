using Newtonsoft.Json;

namespace CarDealer.DTO
{
    public class ImportCarDTO
    {
        [JsonProperty("make")]
        public string Make { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("travelledDistance")]
        public long TravelledDistance { get; set; }

        [JsonProperty("partsId")]
        public ImportCarPartDTO[] PartsId { get; set; }
    }

    public class ImportCarPartDTO
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
