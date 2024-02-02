using System;
using System.Text.Json.Serialization;

namespace AirportAPI.Models;

public sealed class Ticket {

    [JsonPropertyName("id"), JsonRequired]
    public int Id { get; set; }

    [JsonPropertyName("adults"), JsonRequired]
    public int AdultsCount { get; set; }

    [JsonPropertyName("children"), JsonRequired]
    public int ChildrenCount { get; set; }
}