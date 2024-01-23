using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

namespace AirportAPI.Endpoints;

public static class Flights {
    public static IEnumerable<Models.Flight> GetFlights(DatabaseConnection db) {
        string query = """
                SELECT *
                FROM flights
                ORDER BY id;
            """;

        using var con = db.Open();
        return con.Query<Models.Flight>(query);
    }

    public static IEnumerable<Models.FlightJoined> GetFlightsJoined(DatabaseConnection db) {
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
        return con.Query<Models.FlightJoined>(query);
    }

    public static IEnumerable<Models.Flight> GetFlights(DatabaseConnection db, int id) {
        string query = """
                SELECT *
                FROM flights
                WHERE flights.id = @Id;
            """;

        using var con = db.Open();
        return con.Query<Models.Flight>(query, new { Id = id });
    }

    public static IEnumerable<Models.FlightJoined> GetFlightsJoined(DatabaseConnection db, int id) {
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
        return con.Query<Models.FlightJoined>(query, new { Id = id });
    }

    public static void MapFlights(this WebApplication app) {
        app.MapGet("/flights", (DatabaseConnection db) => Results.Ok(GetFlights(db)))
        .WithName("Get Flights")
        .WithOpenApi()
        .Produces<IEnumerable<Models.Flight>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/flights/joined", (DatabaseConnection db) => Results.Ok(GetFlightsJoined(db)))
        .WithName("Get Flights Joined")
        .WithOpenApi()
        .Produces<IEnumerable<Models.FlightJoined>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/flights/{id}", (int id, DatabaseConnection db) => Results.Ok(GetFlights(db, id)))
        .WithName("Get Flight")
        .WithOpenApi()
        .Produces<IEnumerable<Models.Flight>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/flights/{id}/joined", (int id, DatabaseConnection db) => Results.Ok(GetFlightsJoined(db, id)))
        .WithName("Get Flight Joined")
        .WithOpenApi()
        .Produces<IEnumerable<Models.FlightJoined>>(StatusCodes.Status200OK, "application/json");

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
