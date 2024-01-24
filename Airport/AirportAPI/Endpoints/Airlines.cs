using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;

namespace AirportAPI.Endpoints;

public static class Airlines {
    public static IEnumerable<Models.Airline> GetAirlines(this DatabaseConnection db) {
        string query = """
                SELECT *
                FROM airlines
                ORDER BY id;
            """;

        using var con = db.Open();
        return con.Query<Models.Airline>(query);
    }

    public static Models.Airline? GetAirline(this DatabaseConnection db, int id) {
        string query = """
                SELECT *
                FROM airlines
                WHERE id = @Id;
            """;

        using var con = db.Open();
        return con.Query<Models.Airline>(query, new { Id = id }).FirstOrDefault();
    }

    public static void MapAirlines(this WebApplication app) {
        app.MapGet("/airlines", (DatabaseConnection db) => Results.Ok(db.GetAirlines()))
        .WithName("Get Airlines")
        .WithTags("Airlines Endpoints")
        .WithOpenApi()
        .Produces<IEnumerable<Models.Airline>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/airlines/{id}", (int id, DatabaseConnection db) => {
            var item = db.GetAirline(id);
            return item is null ? Results.NotFound() : Results.Ok(item);
        })
        .WithName("Get Airline")
        .WithTags("Airlines Endpoints")
        .WithOpenApi()
        .Produces<Models.Airline?>(StatusCodes.Status200OK, "application/json");
    }
}
