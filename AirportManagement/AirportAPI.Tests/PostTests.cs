namespace AirportAPI.Tests;

[TestClass]
public class PostTests {
    private const string ApiAddressEnv = "API_ADDRESS";

    private HttpClient _client = null!;

    [TestInitialize]
    public void Initialize() => _client = new() { BaseAddress = new($"http://{Environment.GetEnvironmentVariable(ApiAddressEnv) ?? "localhost"}:5000") };

    [TestCleanup]
    public void Cleanup() => _client?.Dispose();

    [TestMethod]
    [DataRow(11), DataRow(12), DataRow(13)]
    public async Task PostAirline(int nameId) {
        Airline airline = new() { Name = $"airline-{nameId}" };
        var response = await _client.PostAsJsonAsync("/airlines", airline);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        var airlines = await _client.GetFromJsonAsync<IEnumerable<Airline>>("/airlines");
        Assert.IsNotNull(airlines);
        Assert.IsTrue(airline.EqualsAny(airlines));
    }

    [TestMethod]
    [DataRow(11), DataRow(12), DataRow(13)]
    public async Task PostCity(int nameId) {
        City city = new() { Name = $"city-{nameId}", Population = nameId * 1000 };
        var response = await _client.PostAsJsonAsync("/cities", city);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        var cities = await _client.GetFromJsonAsync<IEnumerable<City>>("/cities");
        Assert.IsNotNull(cities);
        Assert.IsTrue(city.EqualsAny(cities));
    }

    [TestMethod]
    [DataRow(1), DataRow(2), DataRow(3)]
    public async Task PostFlight(int airlineId) {
        Flight flight = new() {
            AirlineId = airlineId,
            OriginId = airlineId * 2 - 1,
            DestinationId = airlineId * 2,
            Distance = airlineId * 400,
            FlightTime = airlineId * 50,
            HufPerKm = airlineId * 6
        };
        var response = await _client.PostAsJsonAsync("/flights", flight);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        var flights = await _client.GetFromJsonAsync<IEnumerable<Flight>>("/flights");
        Assert.IsNotNull(flights);
        Assert.IsTrue(flight.EqualsAny(flights));
    }

    [TestMethod]
    [DataRow(100), DataRow(200), DataRow(300)]
    public async Task PostInvalidFlight(int airlineId) {
        Flight flight = new() {
            AirlineId = airlineId,
            OriginId = airlineId * 2 - 1,
            DestinationId = airlineId * 2,
            Distance = airlineId * 300,
            FlightTime = airlineId * 30,
            HufPerKm = airlineId * 3
        };
        var response = await _client.PostAsJsonAsync("/flights", flight);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    [DataRow("/airline"), DataRow("/city"), DataRow("/flight")]
    public async Task PostWrongPath(string path) {
        var response = await _client.PostAsync(path, null);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
