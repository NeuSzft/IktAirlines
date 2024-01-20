using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

namespace AirportAPI.Endpoints;

public static class Flights {
    public static IEnumerable<dynamic> GetFlights(DatabaseConnection db) {
        string query = """
                SELECT
                    flights.id          AS "id",
            	    airlines.name       AS "airline",
            	    o.name              AS "origin_city",
            	    o.population        AS "origin_city_population",
            	    d.name              AS "destination_city",
            	    d.population        AS "destination_city_population",
            	    flights.distance    AS "distance",
            	    flights.flight_time AS "flight_time",
            	    flights.huf_per_km  AS "huf_per_km",
            	    flights.distance    AS "distance"
                FROM flights
                INNER JOIN airlines ON airlines.id = flights.airline_id
                INNER JOIN cities o ON o.id = flights.origin_id
                INNER JOIN cities d ON d.id = flights.destination_id
                ORDER BY flights.id;
            """;

        using var con = db.Open();
        return con.Query(query);
    }

    public static IEnumerable<dynamic> GetFlight(DatabaseConnection db, int id) {
        string query = """
                SELECT
            	    airlines.name       AS "airline",
            	    o.name              AS "origin_city",
            	    o.population        AS "origin_city_population",
            	    d.name              AS "destination_city",
            	    d.population        AS "destination_city_population",
            	    flights.distance    AS "distance",
            	    flights.flight_time AS "flight_time",
            	    flights.huf_per_km  AS "huf_per_km",
            	    flights.distance    AS "distance"
                FROM flights
                INNER JOIN airlines ON airlines.id = flights.airline_id
                INNER JOIN cities o ON o.id = flights.origin_id
                INNER JOIN cities d ON d.id = flights.destination_id
                WHERE flights.id = @Id;
            """;

        using var con = db.Open();
        return con.Query(query, new { Id = id });
    }

    public static void MapFlights(this WebApplication app) {
        app.MapGet("/flights", (DatabaseConnection db) => Results.Ok(GetFlights(db)))
        .WithName("Get Flights")
        .WithOpenApi();

        app.MapGet("/flights/{id}", (int id, DatabaseConnection db) => Results.Ok(GetFlight(db, id)))
        .WithName("Get Flight")
        .WithOpenApi();

        app.MapPost("/flights", (DatabaseConnection db) => Results.StatusCode(StatusCodes.Status501NotImplemented))
        .WithName("Post Flight")
        .WithOpenApi();

        app.MapPut("/flights/{id}", (int id, DatabaseConnection db) => Results.StatusCode(StatusCodes.Status501NotImplemented))
        .WithName("Put Flight")
        .WithOpenApi();

        app.MapDelete("/flights/{id}", (int id, DatabaseConnection db) => Results.StatusCode(StatusCodes.Status501NotImplemented))
        .WithName("Del Flight")
        .WithOpenApi();
    }
}
