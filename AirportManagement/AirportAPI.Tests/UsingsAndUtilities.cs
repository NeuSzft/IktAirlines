global using AirportAPI.Models;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using System.Net;
global using System.Net.Http.Json;

using System.Reflection;

namespace AirportAPI.Tests;

internal static class Utilities {
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
