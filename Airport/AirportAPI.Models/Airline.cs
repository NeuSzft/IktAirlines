using System.Text.Json.Serialization;

namespace AirportAPI.Models;

public sealed class Airline : IdModel {

    [JsonPropertyName("name"), JsonRequired]
    public string Name { get; set; } = null!;

    public override string ToString() => $"{Name} ({Id})";
}
