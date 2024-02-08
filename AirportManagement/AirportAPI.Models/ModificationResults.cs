using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirportAPI.Models.Testing;

public class ModificationResults(int affected, IEnumerable<Airline> airlines, IEnumerable<City> cities, IEnumerable<Flight> flights) {
    [JsonPropertyName("rows_affected"), JsonRequired]
    public int RowsAffected { get; set; } = affected;

    [JsonPropertyName("airlines"), JsonRequired]
    public IEnumerable<Airline> Airlines { get; set; } = airlines;

    [JsonPropertyName("cities"), JsonRequired]
    public IEnumerable<City> Cities { get; set; } = cities;

    [JsonPropertyName("flights"), JsonRequired]
    public IEnumerable<Flight> Flights { get; set; } = flights;
}
