using System.Text.Json.Serialization;

namespace AirportAPI.Models;

public sealed class City : IdModel {

    [JsonPropertyName("name"), JsonRequired]
    public string Name { get; set; } = null!;

    [JsonPropertyName("population"), JsonRequired]
    public int Population { get; set; }

    public override string ToString() => $"{Name} ({Id})";
}
