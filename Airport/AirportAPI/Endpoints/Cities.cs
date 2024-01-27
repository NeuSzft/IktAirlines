using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;

namespace AirportAPI.Endpoints;

public static class Cities {
    public static IEnumerable<Models.City> GetCities(this DatabaseConnection db) {
        string query = """
            SELECT *
            FROM cities
            ORDER BY id;
        """;

        using var con = db.Open();
        return con.Query<Models.City>(query);
    }

    public static Models.City? GetCity(this DatabaseConnection db, int id) {
        string query = """
            SELECT *
            FROM cities
            WHERE id = @Id;
        """;

        using var con = db.Open();
        return con.Query<Models.City>(query, new { Id = id }).FirstOrDefault();
    }

    public static int PostCity(this DatabaseConnection db, Models.City city) {
        const string query = """
            INSERT INTO cities(name, population)
            VALUES (@Name, @Population);
        """;

        using var con = db.Open();
        return con.Execute(query, new { city.Name, city.Population });
    }

    public static int PutCity(this DatabaseConnection db, int id, Models.City city) {
        const string query = """
            UPDATE cities
            SET name=@Name, population=@Population
            WHERE id = @Id;
        """;

        using var con = db.Open();
        return con.Execute(query, new { Id = id, city.Name, city.Population });
    }

    public static int DelCity(this DatabaseConnection db, int id) {
        const string query = """
            DELETE FROM cities
            WHERE id = @Id;
        """;

        using var con = db.Open();
        return con.Execute(query, new { Id = id });
    }

    public static void MapCities(this WebApplication app) {
        app.MapGet("/cities", (DatabaseConnection db) => Results.Ok(db.GetCities()))
       .WithName("Get Cities")
       .WithTags("Cities Endpoints")
       .WithOpenApi()
       .Produces<IEnumerable<Models.City>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/cities/{id}", (int id, DatabaseConnection db) => {
            var item = db.GetCity(id);
            return item is null ? Results.NotFound() : Results.Ok(item);
        })
        .WithName("Get City")
        .WithTags("Cities Endpoints")
        .WithOpenApi()
        .Produces<Models.City?>(StatusCodes.Status200OK, "application/json");

        app.MapPost("/cities", (Models.City city, DatabaseConnection db) => db.PostCity(city) > 0 ? Results.Created() : Results.BadRequest())
        .WithName("Post City")
        .WithTags("Cities Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/cities/{id}", (int id, Models.City city, DatabaseConnection db) => db.PutCity(id, city) > 0 ? Results.Ok() : Results.NotFound())
        .WithName("Put City")
        .WithTags("Cities Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        app.MapDelete("/cities/{id}", (int id, DatabaseConnection db) => db.DelCity(id) > 0 ? Results.Ok() : Results.NotFound())
        .WithName("Del City")
        .WithTags("Cities Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
