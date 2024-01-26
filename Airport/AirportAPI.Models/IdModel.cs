using System;
using System.Text.Json.Serialization;

namespace AirportAPI.Models;

public abstract class IdModel : ICloneable {
    [JsonPropertyName("id")]
    public int Id { get; set; }

    public object Clone() => MemberwiseClone();
}
