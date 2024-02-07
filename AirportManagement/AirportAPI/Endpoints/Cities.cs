using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using System.Collections.Generic;
using System.Linq;

namespace AirportAPI.Endpoints;

public static class Cities {
    public static IEnumerable<Models.City> GetCities(this NpgsqlConnection connection) {
        string query = """
            SELECT *
            FROM cities
            ORDER BY id;
        """;

        return connection.Query<Models.City>(query);
    }

    public static Models.City? GetCity(this NpgsqlConnection connection, int id) {
        string query = """
            SELECT *
            FROM cities
            WHERE id = @Id;
        """;

        return connection.Query<Models.City>(query, new { Id = id }).FirstOrDefault();
    }

    public static int PostCity(this NpgsqlConnection connection, Models.City city) {
        const string query = """
            INSERT INTO cities(name, population)
            VALUES (@Name, @Population);
        """;

        return connection.Execute(query, new { city.Name, city.Population });
    }

    public static int PutCity(this NpgsqlConnection connection, int id, Models.City city) {
        const string query = """
            UPDATE cities
            SET name=@Name, population=@Population
            WHERE id = @Id;
        """;

        return connection.Execute(query, new { Id = id, city.Name, city.Population });
    }

    public static int DelCity(this NpgsqlConnection connection, int id) {
        const string query = """
            DELETE FROM cities
            WHERE id = @Id;
        """;

        return connection.Execute(query, new { Id = id });
    }

    public static void MapCities(this WebApplication app) {
        app.MapGet("/cities", (DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            return Results.Ok(connection.GetCities());
        })
        .WithSummary("Get all cities.")
        .WithTags("Cities Endpoints")
        .WithOpenApi()
        .Produces<IEnumerable<Models.City>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/cities/{id}", (int id, DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            var item = connection.GetCity(id);
            return item is null ? Results.NotFound() : Results.Ok(item);
        })
        .WithSummary("Get a city by id.")
        .WithTags("Cities Endpoints")
        .WithOpenApi()
        .Produces<Models.City?>(StatusCodes.Status200OK, "application/json")
        .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/cities", (Models.City city, DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            return connection.PostCity(city) > 0 ? Results.Created() : Results.BadRequest();
        })
        .WithSummary("Add a new city.")
        .WithTags("Cities Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/cities/{id}", (int id, Models.City city, DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            return connection.PutCity(id, city) > 0 ? Results.Ok() : Results.NotFound();
        })
        .WithSummary("Overwrite an existing city.")
        .WithTags("Cities Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        app.MapDelete("/cities/{id}", (int id, DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            return connection.DelCity(id) > 0 ? Results.Ok() : Results.NotFound();
        })
        .WithSummary("Delete an existing city.")
        .WithTags("Cities Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
