using System.Text.Json.Serialization;

namespace AirportAPI.Models;

public class Flight {
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("airline_id")]
    public int AirlineId { get; set; }

    [JsonPropertyName("origin_id")]
    public int OriginId { get; set; }

    [JsonPropertyName("destination_id")]
    public int DestinationId { get; set; }

    [JsonPropertyName("distance")]
    public int Distance { get; set; }

    [JsonPropertyName("flight_time")]
    public int FlightTime { get; set; }

    [JsonPropertyName("huf_per_km")]
    public int HufPerKm { get; set; }
}
