using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace AirportAPI.Endpoints;

public static class Other {
    public static int? GetNextId(this DatabaseConnection db, string table) {
        string query = $"SELECT last_value FROM {table}_id_seq;";

        using var con = db.Open();
        return con.Query<int>(query).FirstOrDefault();
    }

    public static void MapOther(this WebApplication app) {
        app.MapGet("/next-id/airlines", (DatabaseConnection db) => {
            int? id = db.GetNextId("airlines");
            return id is null ? Results.NotFound() : Results.Ok(id + 1);
        })
        .WithName("Get next available Airline Id")
        .WithTags("Other Endpoints")
        .WithOpenApi()
        .Produces<int>(StatusCodes.Status200OK, "text/plain")
        .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/next-id/cities", (DatabaseConnection db) => {
            int? id = db.GetNextId("cities");
            return id is null ? Results.NotFound() : Results.Ok(id + 1);
        })
        .WithName("Get next available City Id")
        .WithTags("Other Endpoints")
        .WithOpenApi()
        .Produces<int>(StatusCodes.Status200OK, "text/plain")
        .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/next-id/flights", (DatabaseConnection db) => {
            int? id = db.GetNextId("flights");
            return id is null ? Results.NotFound() : Results.Ok(id + 1);
        })
        .WithName("Get next available Flight Id")
        .WithTags("Other Endpoints")
        .WithOpenApi()
        .Produces<int>(StatusCodes.Status200OK, "text/plain")
        .Produces(StatusCodes.Status404NotFound);
    }
}
