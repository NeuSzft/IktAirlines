namespace AirportAPI.Tests;

[TestClass]
public class DeleteTests {
    private HttpClient _client = null!;

    [TestInitialize]
    public void Initialize() => _client = Utilities.Initialize();

    [TestCleanup]
    public void Cleanup() => Utilities.Cleanup(_client);

    [TestMethod]
    [DataRow(4)]
    public async Task DeleteAirline(int id) {
        var response = await _client.DeleteAsync($"/airlines/{id}");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        response = await _client.GetAsync($"/airlines/{id}");
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [DataRow(7), DataRow(8)]
    public async Task DeleteCity(int id) {
        var response = await _client.DeleteAsync($"/cities/{id}");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        response = await _client.GetAsync($"/cities/{id}");
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [DataRow(1), DataRow(2), DataRow(3)]
    public async Task DeleteFlight(int id) {
        var response = await _client.DeleteAsync($"/flights/{id}");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        response = await _client.GetAsync($"/flights/{id}");
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [DataRow(1), DataRow(2), DataRow(3)]
    public async Task FailToDeleteAirline(int id) {
        var response = await _client.DeleteAsync($"/airlines/{id}");
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        response = await _client.GetAsync($"/airlines/{id}");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [DataRow(2), DataRow(3), DataRow(5)]
    public async Task FailToDeleteCity(int id) {
        var response = await _client.DeleteAsync($"/cities/{id}");
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        response = await _client.GetAsync($"/cities/{id}");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [DataRow("/airlines/17"), DataRow("/cities/17"), DataRow("/flights/17")]
    public async Task DeleteNonExistentItems(string path) {
        var response = await _client.DeleteAsync(path);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    [DataRow("/airline"), DataRow("/city"), DataRow("/flight")]
    public async Task DeleteWrongPath(string path) {
        var response = await _client.DeleteAsync(path);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
