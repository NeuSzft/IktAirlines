using System.Collections.Generic;

namespace AirportAPI.Models.Testing;

public class ModificationResults(int affected, IEnumerable<Airline> airlines, IEnumerable<City> cities, IEnumerable<Flight> flights) {
    public int RowsAffected { get; set; } = affected;

    public IEnumerable<Airline> Airlines { get; set; } = airlines;

    public IEnumerable<City> Cities { get; set; } = cities;

    public IEnumerable<Flight> Flights { get; set; } = flights;
}
