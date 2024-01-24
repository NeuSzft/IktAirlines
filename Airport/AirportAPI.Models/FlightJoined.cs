using System.Text.Json.Serialization;

namespace AirportAPI.Models;

public class FlightJoined {
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("airline"), JsonRequired]
    public string Airline { get; set; } = null!;

    [JsonPropertyName("origin_city"), JsonRequired]
    public string OriginCity { get; set; } = null!;

    [JsonPropertyName("origin_city_population"), JsonRequired]
    public int OriginCityPopulation { get; set; }

    [JsonPropertyName("destination_city"), JsonRequired]
    public string DestinationCity { get; set; } = null!;

    [JsonPropertyName("destination_city_population"), JsonRequired]
    public int DestinationCityPopulation { get; set; }

    [JsonPropertyName("distance"), JsonRequired]
    public int Distance { get; set; }

    [JsonPropertyName("flight_time"), JsonRequired]
    public int FlightTime { get; set; }

    [JsonPropertyName("huf_per_km"), JsonRequired]
    public int HufPerKm { get; set; }

    public override string ToString() => $"{OriginCity} - {DestinationCity} ({Id})";
}
