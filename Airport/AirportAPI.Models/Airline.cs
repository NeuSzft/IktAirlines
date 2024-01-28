using System;
using System.Text.Json.Serialization;

namespace AirportAPI.Models;

public sealed class Airline : IdModel, IEquatable<Airline> {

    [JsonPropertyName("name"), JsonRequired]
    public string Name { get; set; } = null!;

    public bool Equals(Airline? other) {
        return other is not null
            && Id == other.Id
            && Name == other.Name;
    }

    public override string ToString() => $"{Name} ({Id})";
}
