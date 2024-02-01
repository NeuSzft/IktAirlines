using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirportAPI.Models;

public abstract class OperationInfo(string operationName, string modelName) {
    [JsonPropertyName("operation_name"), JsonRequired]
    public string OperationName { get; set; } = operationName;

    [JsonPropertyName("model_name"), JsonRequired]
    public string ModelName { get; set; } = modelName;

    [JsonPropertyName("id"), JsonRequired]
    public int Id { get; set; }

    [JsonPropertyName("item"), JsonRequired]
    public object Item { get; set; } = null!;
}

public abstract record Operation {
    public static Operation FromJson(JsonElement operationInfoElement) {
        JsonElement operationName = operationInfoElement.GetProperty("operation_name");
        JsonElement modelName = operationInfoElement.GetProperty("model_name");
        JsonElement id = operationInfoElement.GetProperty("id");
        JsonElement item = operationInfoElement.GetProperty("item");

        Assembly assembly = typeof(Operation).Assembly;
        string? assemblyName = assembly.GetName().Name;

        string operationFullName = $"{assemblyName}.{operationName.GetString()}";
        Type? operationType = assembly.GetType(operationFullName);
        if (operationType is null)
            throw new TypeLoadException(operationFullName);

        string modelFullName = $"{assemblyName}.{modelName.GetString()}";
        Type? modelType = assembly.GetType(modelFullName);
        if (modelType is null)
            throw new TypeLoadException(modelFullName);

        Type type = operationType.MakeGenericType(modelType);
        Operation operation = (Operation)Activator.CreateInstance(type)!;

        type.GetProperty("Id")?.SetValue(operation, id.GetInt32());
        type.GetProperty("Item")?.SetValue(operation, item.Deserialize(modelType));

        return operation;
    }

    public static IEnumerable<Operation> FromJson(JsonDocument document) {
        if (document.RootElement.ValueKind == JsonValueKind.Array)
            return document.RootElement.EnumerateArray().Select(FromJson);
        return [];
    }
}

public sealed record AddOperation<T> : Operation where T : IdModel {
    public T Item { get; internal set; } = null!;
}

public sealed record UpdateOperation<T> : Operation where T : IdModel {
    public int Id { get; internal set; }
    public T Item { get; internal set; } = null!;
}

public sealed record RemoveOperation<T> : Operation where T : IdModel {
    public int Id { get; internal set; }
}

public class UnknownOperationException(object? operation) : Exception {
    public override string Message => $"{operation?.GetType()} is an unknown operation";
}
