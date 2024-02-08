global using AirportAPI.Models;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using System.Net;
global using System.Net.Http.Json;
using System.Diagnostics;
using System.Reflection;

namespace AirportAPI.Tests;

[TestClass]
public static class Utilities {
    private const string ApiAddressEnv = "API_ADDRESS";
    private const string DbHostEnv = "DB_HOST";
    private const string DbNameEnv = "DB_NAME";
    private const string DbUserEnv = "DB_USER";

    [AssemblyInitialize]
    public static void DumpDatabase(TestContext context) {
        StartProcess(
            "pg_dump",
            "-v",
            "-h",
            Environment.GetEnvironmentVariable(DbHostEnv)!,
            "-U",
            Environment.GetEnvironmentVariable(DbUserEnv)!,
            "-F",
            "c",
            "-f",
            "dbdump",
            Environment.GetEnvironmentVariable(DbNameEnv)!
        );
    }

    private static void StartProcess(string process, params string[] args) {
        ProcessStartInfo startInfo = new(process, args) {
            UseShellExecute = false,
            RedirectStandardError = true
        };

        Process proc = new() { StartInfo = startInfo };

        proc.Start();
        string error = proc.StandardError.ReadToEnd();
        proc.WaitForExit();

        if (proc.ExitCode != 0)
            throw new ApplicationException($"!!! {process} exited with {proc.ExitCode} !!!\n\n{error}");
    }

    public static HttpClient Initialize() {
        return new() { BaseAddress = new($"http://{Environment.GetEnvironmentVariable(ApiAddressEnv)}:5000") };
    }

    public static void Cleanup(HttpClient? client) {
        client?.Dispose();

        StartProcess(
            "pg_restore",
            "-v",
            "-h",
            Environment.GetEnvironmentVariable(DbHostEnv)!,
            "-U",
            Environment.GetEnvironmentVariable(DbUserEnv)!,
            "-d",
            Environment.GetEnvironmentVariable(DbNameEnv)!,
            "-c",
            "dbdump"
        );
    }

    public static bool AreEqual<T>(this T model, T other) where T : IdModel {
        foreach (PropertyInfo info in typeof(T).GetProperties().Where(x => x.Name != "Id"))
            if (!info.GetValue(model)?.Equals(info.GetValue(other)) ?? true)
                return false;
        return true;
    }

    public static bool EqualsAny<T>(this T model, IEnumerable<T> others) where T : IdModel {
        foreach (T item in others)
            if (AreEqual(model, item))
                return true;
        return false;
    }
}
