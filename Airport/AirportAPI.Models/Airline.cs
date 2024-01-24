using System.Text.Json.Serialization;

namespace AirportAPI.Models;

public class Airline {
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name"), JsonRequired]
    public string Name { get; set; } = null!;
}
