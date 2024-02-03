using AirportAPI.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;

namespace AirportApp;

internal sealed class RequestHelper(string baseAddress) : IDisposable {
    public Uri BaseAddress => _client.BaseAddress!;

    private HttpClient _client = new() { BaseAddress = new(baseAddress) };

    public async Task<IEnumerable<T>?> Get<T>(string path) {
        try {
            return await _client.GetFromJsonAsync<IEnumerable<T>>(path);
        } catch (Exception e) {
            ShowRequestError("GET", path, e);
            return null;
        }
    }

    public async Task<bool> Post<T>(string path, T obj) {
        try {
            return (await _client.PostAsJsonAsync(path, obj)).IsSuccessStatusCode;
        } catch (Exception e) {
            ShowRequestError("POST", path, e);
            return false;
        }
    }

    public async Task<bool> Put<T>(string path, T obj) {
        try {
            return (await _client.PutAsJsonAsync(path, obj)).IsSuccessStatusCode;
        } catch (Exception e) {
            ShowRequestError("PUT", path, e);
            return false;
        }
    }

    public async Task<bool> Delete(string path) {
        try {
            return (await _client.DeleteAsync(path)).IsSuccessStatusCode;
        } catch (Exception e) {
            ShowRequestError("DELETE", path, e);
            return false;
        }
    }

    public async Task<int?> NextId(string path) {
        try {
            string value = await _client.GetStringAsync(path);
            return int.Parse(value);
        } catch (Exception e) {
            ShowRequestError("GET", path, e);
            return null;
        }
    }

    public async Task<bool> Ping() {
        try {
            return (await _client.GetAsync("/ping")).IsSuccessStatusCode;
        } catch {
            return false;
        }
    }

    public async Task<string> Modify(IEnumerable<OperationInfo> operations) {
        try {
            HttpResponseMessage response = await _client.PatchAsJsonAsync("/modify", operations);
            string content = await response.Content.ReadAsStringAsync();
            string result = response.IsSuccessStatusCode ? $"Successfully performed {int.Parse(content)} operations" : $"\n{content}";
            return $"{response.StatusCode} ({(int)response.StatusCode}) {result}";
        } catch (Exception e) {
            ShowRequestError("DELETE", "/modify", e);
            return e.Message;
        }
    }

    public void SetBaseAddress(string baseAddress) {
        _client.Dispose();
        _client = new() { BaseAddress = new(baseAddress) };
    }

    public void Dispose() => _client.Dispose();

    private void ShowRequestError(string httpMethod, string path, Exception e) {
        MessageBox.Show(e.ToString(), $"{httpMethod} {new Uri(BaseAddress, path)}", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
