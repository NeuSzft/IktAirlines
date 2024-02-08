using System;
using System.Text.Json.Serialization;

namespace AirportAPI.Models;

public sealed class City : IdModel, IEquatable<City> {

    [JsonPropertyName("name"), JsonRequired]
    public string Name { get; set; } = null!;

    [JsonPropertyName("population"), JsonRequired]
    public int Population { get; set; }

    public bool Equals(City? other) {
        return other is not null
            && Id == other.Id
            && Name == other.Name
            && Population == other.Population;
    }

    public override string ToString() => $"{Name} ({Id})";
}
