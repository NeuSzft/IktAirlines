using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

namespace AirportAPI.Endpoints;

public static class Airlines {
    public static IEnumerable<Models.Airline> GetAirlines(DatabaseConnection db) {
        string query = """
                SELECT *
                FROM airlines
                ORDER BY id;
            """;

        using var con = db.Open();
        return con.Query<Models.Airline>(query);
    }

    public static IEnumerable<Models.Airline> GetAirline(DatabaseConnection db, int id) {
        string query = """
                SELECT name
                FROM airlines
                WHERE id = @Id;
            """;

        using var con = db.Open();
        return con.Query<Models.Airline>(query, new { Id = id });
    }

    public static void MapAirlines(this WebApplication app) {
        app.MapGet("/airlines", (DatabaseConnection db) => Results.Ok(GetAirlines(db)))
        .WithName("Get Airlines")
        .WithOpenApi();

        app.MapGet("/airlines/{id}", (int id, DatabaseConnection db) => Results.Ok(GetAirline(db, id)))
        .WithName("Get Airline")
        .WithOpenApi();

        app.MapPost("/airlines", (DatabaseConnection db) => Results.StatusCode(StatusCodes.Status501NotImplemented))
        .WithName("Post Airline")
        .WithOpenApi();

        app.MapPut("/airlines/{id}", (int id, DatabaseConnection db) => Results.StatusCode(StatusCodes.Status501NotImplemented))
        .WithName("Put Airline")
        .WithOpenApi();

        app.MapDelete("/airlines/{id}", (int id, DatabaseConnection db) => Results.StatusCode(StatusCodes.Status501NotImplemented))
        .WithName("Del Airline")
        .WithOpenApi();
    }
}
