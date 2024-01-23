using System.Text.Json.Serialization;

namespace AirportAPI.Models;

public class FlightJoined {
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("airline")]
    public string Airline { get; set; } = null!;

    [JsonPropertyName("origin_city")]
    public string OriginCity { get; set; } = null!;

    [JsonPropertyName("origin_city_population")]
    public int OriginCityPopulation { get; set; }

    [JsonPropertyName("destination_city")]
    public string DestinationCity { get; set; } = null!;

    [JsonPropertyName("destination_city_population")]
    public int DestinationCityPopulation { get; set; }

    [JsonPropertyName("distance")]
    public int Distance { get; set; }

    [JsonPropertyName("flight_time")]
    public int FlightTime { get; set; }

    [JsonPropertyName("huf_per_km")]
    public int HufPerKm { get; set; }
}
