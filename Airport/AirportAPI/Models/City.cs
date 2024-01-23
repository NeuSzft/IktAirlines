using System.Text.Json.Serialization;

namespace AirportAPI.Models;

public class City {
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name"), JsonRequired]
    public string Name { get; set; } = null!;

    [JsonPropertyName("population"), JsonRequired]
    public int Population { get; set; }
}
