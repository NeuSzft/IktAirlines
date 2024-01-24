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

    public static void MapCities(this WebApplication app) {
        app.MapGet("/cities", (DatabaseConnection db) => Results.Ok(db.GetCities()))
       .WithName("Get Cities")
       .WithTags("City Endpoints")
       .WithOpenApi()
       .Produces<IEnumerable<Models.City>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/cities/{id}", (int id, DatabaseConnection db) => {
            var item = db.GetCity(id);
            return item is null ? Results.NotFound() : Results.Ok(item);
        })
        .WithName("Get City")
        .WithTags("City Endpoints")
        .WithOpenApi()
        .Produces<Models.City?>(StatusCodes.Status200OK, "application/json");
    }
}
