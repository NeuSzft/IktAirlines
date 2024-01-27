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

    public static int PostAirline(this DatabaseConnection db, Models.Airline airline) {
        const string query = """
            INSERT INTO airlines(name)
            VALUES (@Name);
        """;

        using var con = db.Open();
        return con.Execute(query, new { airline.Name });
    }

    public static int PutAirline(this DatabaseConnection db, int id, Models.Airline airline) {
        const string query = """
            UPDATE airlines
            SET name=@Name
            WHERE id = @Id;
        """;

        using var con = db.Open();
        return con.Execute(query, new { Id = id, airline.Name });
    }

    public static int DelAirline(this DatabaseConnection db, int id) {
        const string query = """
            DELETE FROM airlines
            WHERE id = @Id;
        """;

        using var con = db.Open();
        return con.Execute(query, new { Id = id });
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

        app.MapPost("/airlines", (Models.Airline airline, DatabaseConnection db) => db.PostAirline(airline) > 0 ? Results.Created() : Results.BadRequest())
        .WithName("Post Airline")
        .WithTags("Airlines Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/airlines/{id}", (int id, Models.Airline airline, DatabaseConnection db) => db.PutAirline(id, airline) > 0 ? Results.Ok() : Results.NotFound())
        .WithName("Put Airline")
        .WithTags("Airlines Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        app.MapDelete("/airlines/{id}", (int id, DatabaseConnection db) => db.DelAirline(id) > 0 ? Results.Ok() : Results.NotFound())
        .WithName("Del Airline")
        .WithTags("Airlines Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
