using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using System.Collections.Generic;
using System.Linq;

namespace AirportAPI.Endpoints;

public static class Airlines {
    public static IEnumerable<Models.Airline> GetAirlines(this NpgsqlConnection connection) {
        string query = """
            SELECT *
            FROM airlines
            ORDER BY id;
        """;

        return connection.Query<Models.Airline>(query);
    }

    public static Models.Airline? GetAirline(this NpgsqlConnection connection, int id) {
        string query = """
            SELECT *
            FROM airlines
            WHERE id = @Id;
        """;

        return connection.Query<Models.Airline>(query, new { Id = id }).FirstOrDefault();
    }

    public static int PostAirline(this NpgsqlConnection connection, Models.Airline airline) {
        const string query = """
            INSERT INTO airlines(name)
            VALUES (@Name);
        """;

        return connection.Execute(query, new { airline.Name });
    }

    public static int PutAirline(this NpgsqlConnection connection, int id, Models.Airline airline) {
        const string query = """
            UPDATE airlines
            SET name=@Name
            WHERE id = @Id;
        """;

        return connection.Execute(query, new { Id = id, airline.Name });
    }

    public static int DelAirline(this NpgsqlConnection connection, int id) {
        const string query = """
            DELETE FROM airlines
            WHERE id = @Id;
        """;

        return connection.Execute(query, new { Id = id });
    }

    public static void MapAirlines(this WebApplication app) {
        app.MapGet("/airlines", (DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            return Results.Ok(connection.GetAirlines());
        })
        .WithSummary("Get all airlines.")
        .WithTags("Airlines Endpoints")
        .WithOpenApi()
        .Produces<IEnumerable<Models.Airline>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/airlines/{id}", (int id, DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            var item = connection.GetAirline(id);
            return item is null ? Results.NotFound() : Results.Ok(item);
        })
        .WithSummary("Get an airline by id.")
        .WithTags("Airlines Endpoints")
        .WithOpenApi()
        .Produces<Models.Airline?>(StatusCodes.Status200OK, "application/json")
        .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/airlines", (Models.Airline airline, DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            return connection.PostAirline(airline) > 0 ? Results.Created() : Results.BadRequest();
        })
        .WithSummary("Add a new airline.")
        .WithTags("Airlines Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/airlines/{id}", (int id, Models.Airline airline, DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            return connection.PutAirline(id, airline) > 0 ? Results.Ok() : Results.NotFound();
        })
        .WithSummary("Overwrite an existing airline.")
        .WithTags("Airlines Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        app.MapDelete("/airlines/{id}", (int id, DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            return connection.DelAirline(id) > 0 ? Results.Ok() : Results.NotFound();
        })
        .WithSummary("Delete an existing airline.")
        .WithTags("Airlines Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
    }
}
