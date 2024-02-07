namespace AirportAPI.Tests;

[TestClass]
public class PutTests {
    private const string ApiAddressEnv = "API_ADDRESS";

    private HttpClient _client = null!;

    [TestInitialize]
    public void Initialize() => _client = new() { BaseAddress = new($"http://{Environment.GetEnvironmentVariable(ApiAddressEnv) ?? "localhost"}:5000") };

    [TestCleanup]
    public void Cleanup() => _client?.Dispose();

    [TestMethod]
    [DataRow(1), DataRow(2), DataRow(3)]
    public async Task PutAirline(int id) {
        Airline airline = new() { Name = $"airline-{id}" };
        var response = await _client.PutAsJsonAsync($"/airlines/{id}", airline);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var fetched = await _client.GetFromJsonAsync<Airline>($"/airlines/{id}");
        Assert.IsNotNull(fetched);
        Assert.IsTrue(airline.AreEqual(fetched));
    }

    [TestMethod]
    [DataRow(2), DataRow(4), DataRow(6)]
    public async Task PutCity(int id) {
        City city = new() { Name = $"city-{id}", Population = id * 2000 };
        var response = await _client.PutAsJsonAsync($"/cities/{id}", city);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var fetched = await _client.GetFromJsonAsync<City>($"/cities/{id}");
        Assert.IsNotNull(fetched);
        Assert.IsTrue(city.AreEqual(fetched));
    }

    [TestMethod]
    [DataRow(1), DataRow(2), DataRow(3)]
    public async Task PutFlight(int id) {
        Flight flight = new() {
            AirlineId = id,
            OriginId = id * 2 - 1,
            DestinationId = id * 2,
            Distance = id * 400,
            FlightTime = id * 50,
            HufPerKm = id * 6
        };
        var response = await _client.PutAsJsonAsync($"/flights/{id}", flight);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var fetched = await _client.GetFromJsonAsync<Flight>($"/flights/{id}");
        Assert.IsNotNull(fetched);
        Assert.IsTrue(flight.AreEqual(fetched));
    }

    [TestMethod]
    [DataRow(1), DataRow(2), DataRow(3)]
    public async Task PutInvalidFlight(int id) {
        Flight flight = new() {
            AirlineId = id,
            OriginId = id * 3,
            DestinationId = id * 30,
            Distance = id * 300,
            FlightTime = id * 30,
            HufPerKm = id * 3
        };
        var response = await _client.PutAsJsonAsync($"/flights/{id}", flight);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    [DataRow(100), DataRow(200), DataRow(300)]
    public async Task PutAirlineWithNonExistentId(int id) {
        Airline airline = new() { Name = $"airline-{id}" };
        var response = await _client.PutAsJsonAsync($"/airlines/{id}", airline);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [DataRow(100), DataRow(200), DataRow(300)]
    public async Task PutCityWithNonExistentId(int id) {
        City city = new() { Name = $"city-{id}", Population = id * 2000 };
        var response = await _client.PutAsJsonAsync($"/cities/{id}", city);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [DataRow(100), DataRow(200), DataRow(300)]
    public async Task PutFlightWithNonExistentId(int id) {
        Flight flight = new() {
            AirlineId = id,
            OriginId = id * 2 - 1,
            DestinationId = id * 2,
            Distance = id * 400,
            FlightTime = id * 50,
            HufPerKm = id * 6
        };
        var response = await _client.PutAsJsonAsync($"/flights/{id}", flight);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [DataRow("/airlines/1"), DataRow("/cities/2"), DataRow("/flights/3")]
    public async Task PutNullItems(string path) {
        var response = await _client.PutAsync(path, null);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
