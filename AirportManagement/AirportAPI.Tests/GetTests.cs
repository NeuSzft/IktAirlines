namespace AirportAPI.Tests;

[TestClass]
public class GetTests {
    private HttpClient _client = null!;

    [TestInitialize]
    public void Initialize() => _client = Utilities.Initialize();

    [TestCleanup]
    public void Cleanup() => Utilities.Cleanup(_client);

    [TestMethod]
    public async Task GetAllAirlines() {
        var airlines = await _client.GetFromJsonAsync<Airline[]>("/airlines");

        Assert.IsNotNull(airlines);
        Assert.AreEqual(4, airlines.Length);
    }

    [TestMethod]
    [DataRow(1), DataRow(2), DataRow(3), DataRow(4)]
    public async Task GetAirlineById(int id) {
        var airline = await _client.GetFromJsonAsync<Airline>($"/airlines/{id}");

        Assert.IsNotNull(airline);
        Assert.AreEqual(new Airline {
            Id = id,
            Name = $"airline-{id}"
        }, airline);
    }

    [TestMethod]
    public async Task GetAllCities() {
        var cities = await _client.GetFromJsonAsync<City[]>("/cities");

        Assert.IsNotNull(cities);
        Assert.AreEqual(8, cities.Length);
    }

    [TestMethod]
    [DataRow(2), DataRow(3), DataRow(5), DataRow(7)]
    public async Task GetCityById(int id) {
        var city = await _client.GetFromJsonAsync<City>($"/cities/{id}");

        Assert.IsNotNull(city);
        Assert.AreEqual(new City {
            Id = id,
            Name = $"city-{id}",
            Population = id * 1000
        }, city);
    }

    [TestMethod]
    public async Task GetAllFlights() {
        var flights = await _client.GetFromJsonAsync<Flight[]>("/flights");

        Assert.IsNotNull(flights);
        Assert.AreEqual(3, flights.Length);
    }

    [TestMethod]
    [DataRow(1), DataRow(2), DataRow(3)]
    public async Task GetFlightById(int id) {
        var flight = await _client.GetFromJsonAsync<Flight>($"/flights/{id}");

        Assert.IsNotNull(flight);
        Assert.AreEqual(new Flight {
            Id = id,
            AirlineId = id,
            OriginId = id * 2 - 1,
            DestinationId = id * 2,
            Distance = id * 300,
            FlightTime = id * 30,
            HufPerKm = id * 3,
        }, flight);
    }

    [TestMethod]
    public async Task GetAllFlightsJoined() {
        var flights = await _client.GetFromJsonAsync<FlightJoined[]>("/flights/joined");

        Assert.IsNotNull(flights);
        Assert.AreEqual(3, flights.Length);
    }

    [TestMethod]
    [DataRow(1), DataRow(2), DataRow(3)]
    public async Task GetFlightJoinedById(int id) {
        var flight = await _client.GetFromJsonAsync<FlightJoined>($"/flights/{id}/joined");

        Assert.IsNotNull(flight);

        int originId = id * 2 - 1;
        int destinationId = id * 2;

        Assert.AreEqual(id, flight.Id);
        Assert.AreEqual($"airline-{id}", flight.Airline);
        Assert.AreEqual($"city-{originId}", flight.OriginCity);
        Assert.AreEqual(originId * 1000, flight.OriginCityPopulation);
        Assert.AreEqual($"city-{destinationId}", flight.DestinationCity);
        Assert.AreEqual(destinationId * 1000, flight.DestinationCityPopulation);
        Assert.AreEqual(id * 300, flight.Distance);
        Assert.AreEqual(id * 30, flight.FlightTime);
        Assert.AreEqual(id * 3, flight.HufPerKm);
    }

    [TestMethod]
    [DataRow("/airlines/17"), DataRow("/cities/17"), DataRow("/flights/17"), DataRow("/flights/17/joined")]
    public async Task GetNonExistentItems(string path) {
        var response = await _client.GetAsync(path);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [DataRow("/airline"), DataRow("/city"), DataRow("/flight")]
    public async Task GetWrongPath(string path) {
        var response = await _client.GetAsync(path);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
