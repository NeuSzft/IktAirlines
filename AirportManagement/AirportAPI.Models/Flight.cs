using System;
using System.Text.Json.Serialization;

namespace AirportAPI.Models;

public sealed class Flight : IdModel, IEquatable<Flight> {

    [JsonPropertyName("airline_id"), JsonRequired]
    public int AirlineId { get; set; }

    [JsonPropertyName("origin_id"), JsonRequired]
    public int OriginId { get; set; }

    [JsonPropertyName("destination_id"), JsonRequired]
    public int DestinationId { get; set; }

    [JsonPropertyName("distance"), JsonRequired]
    public int Distance { get; set; }

    [JsonPropertyName("flight_time"), JsonRequired]
    public int FlightTime { get; set; }

    [JsonPropertyName("huf_per_km"), JsonRequired]
    public int HufPerKm { get; set; }

    public bool Equals(Flight? other) {
        return other is not null
            && Id == other.Id
            && AirlineId == other.AirlineId
            && OriginId == other.OriginId
            && DestinationId == other.DestinationId
            && Distance == other.Distance
            && FlightTime == other.FlightTime
            && HufPerKm == other.HufPerKm;
    }

    public override string ToString() => $"from {OriginId} to {DestinationId} ({Id})";
}
