using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace AirportApp;

internal sealed record RequestResult(HttpStatusCode Status, string Message);

internal sealed class RequestHelper(string baseAddress) : IDisposable {
    public class ResponseException(string message) : Exception {
        public override string Message => message;
    }

    public Uri BaseAddress => _client.BaseAddress!;

    private HttpClient _client = new() { BaseAddress = new(baseAddress) };

    public async Task<IEnumerable<T>?> Get<T>(string path) {
        HttpResponseMessage response = await _client.GetAsync(path);
        if (!response.IsSuccessStatusCode)
            throw new ResponseException(await response.Content.ReadAsStringAsync());
        using Stream stream = await response.Content.ReadAsStreamAsync();
        return JsonSerializer.Deserialize<IEnumerable<T>>(stream);
    }

    public async Task Post<T>(string path, T obj) {
        HttpResponseMessage response = await _client.PostAsJsonAsync(path, obj);
        if (!response.IsSuccessStatusCode)
            throw new ResponseException(await response.Content.ReadAsStringAsync());
    }

    public async Task Put<T>(string path, T obj) {
        HttpResponseMessage response = await _client.PutAsJsonAsync(path, obj);
        if (!response.IsSuccessStatusCode)
            throw new ResponseException(await response.Content.ReadAsStringAsync());
    }

    public async Task Delete(string path) {
        HttpResponseMessage response = await _client.DeleteAsync(path);
        if (!response.IsSuccessStatusCode)
            throw new ResponseException(await response.Content.ReadAsStringAsync());
    }

    public async Task<int> NextId(string path) {
        HttpResponseMessage response = await _client.GetAsync(path);
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new ResponseException(content);
        return int.Parse(content);
    }

    public async Task<bool> Ping() {
        try {
            return (await _client.GetAsync("/ping")).IsSuccessStatusCode;
        } catch {
            return false;
        }
    }

    public void SetBaseAddress(string baseAddress) {
        _client.Dispose();
        _client = new() { BaseAddress = new(baseAddress) };
    }

    public void Dispose() => _client.Dispose();
}
