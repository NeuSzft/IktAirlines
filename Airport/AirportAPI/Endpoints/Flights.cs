using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

namespace AirportAPI.Endpoints;

public static class Flights {
    public static IEnumerable<Models.Flight> GetFlights(this DatabaseConnection db) {
        string query = """
                SELECT
                    id                  AS "Id",
            	    airline_id          AS "AirlineId",
            	    origin_id           AS "OriginId",
            	    destination_id      AS "DestinationId",
            	    flights.flight_time AS "FlightTime",
            	    flights.huf_per_km  AS "HufPerKm",
            	    flights.distance    AS "Distance"
                FROM flights
                ORDER BY id;
            """;

        using var con = db.Open();
        return con.Query<Models.Flight>(query);
    }

    public static IEnumerable<Models.FlightJoined> GetFlightsJoined(this DatabaseConnection db) {
        string query = """
                SELECT
                    flights.id          AS "Id",
            	    airlines.name       AS "Airline",
            	    o.name              AS "OriginCity",
            	    o.population        AS "OriginCityPopulation",
            	    d.name              AS "DestinationCity",
            	    d.population        AS "DestinationCityPopulation",
            	    flights.distance    AS "Distance",
            	    flights.flight_time AS "FlightTime",
            	    flights.huf_per_km  AS "HufPerKm",
            	    flights.distance    AS "Distance"
                FROM flights
                INNER JOIN airlines ON airlines.id = flights.airline_id
                INNER JOIN cities o ON o.id = flights.origin_id
                INNER JOIN cities d ON d.id = flights.destination_id
                ORDER BY flights.id;
            """;

        using var con = db.Open();
        return con.Query<Models.FlightJoined>(query);
    }

    public static IEnumerable<Models.Flight> GetFlights(this DatabaseConnection db, int id) {
        string query = """
                SELECT
                    id                  AS "Id",
            	    airline_id          AS "Airline",
            	    origin_id           AS "OriginId",
            	    destination_id      AS "DestinationId",
            	    flights.flight_time AS "FlightTime",
            	    flights.huf_per_km  AS "HufPerKm",
            	    flights.distance    AS "Distance"
                FROM flights
                WHERE flights.id = @Id;
            """;

        using var con = db.Open();
        return con.Query<Models.Flight>(query, new { Id = id });
    }

    public static IEnumerable<Models.FlightJoined> GetFlightsJoined(this DatabaseConnection db, int id) {
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

    public static int PostFlights(this DatabaseConnection db, Models.Flight flight) {
        string query = """
                INSERT INTO flights(airline_id, origin_id, destination_id, distance, flight_time, huf_per_km)
                VALUES (@AirlineId, @OriginId, @DestinationId, @Distance, @FlightTime, @HufPerKm);
            """;

        using var con = db.Open();
        return con.Execute(query, new { flight.AirlineId, flight.OriginId, flight.DestinationId, flight.Distance, flight.FlightTime, flight.HufPerKm });
    }

    public static void MapFlights(this WebApplication app) {
        app.MapGet("/flights", (DatabaseConnection db) => Results.Ok(db.GetFlights()))
        .WithName("Get Flights")
        .WithOpenApi()
        .Produces<IEnumerable<Models.Flight>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/flights/joined", (DatabaseConnection db) => Results.Ok(db.GetFlightsJoined()))
        .WithName("Get Flights Joined")
        .WithOpenApi()
        .Produces<IEnumerable<Models.FlightJoined>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/flights/{id}", (int id, DatabaseConnection db) => Results.Ok(db.GetFlights(id)))
        .WithName("Get Flight")
        .WithOpenApi()
        .Produces<IEnumerable<Models.Flight>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/flights/{id}/joined", (int id, DatabaseConnection db) => Results.Ok(db.GetFlightsJoined(id)))
        .WithName("Get Flight Joined")
        .WithOpenApi()
        .Produces<IEnumerable<Models.FlightJoined>>(StatusCodes.Status200OK, "application/json");

        app.MapPost("/flights", (Models.Flight flight, DatabaseConnection db) => {
            db.PostFlights(flight);
            return Results.Created();
        })
        .WithName("Post Flight")
        .WithOpenApi()
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status409Conflict);

        app.MapPut("/flights/{id}", (int id, DatabaseConnection db) => Results.StatusCode(StatusCodes.Status501NotImplemented))
        .WithName("Put Flight")
        .WithOpenApi();

        app.MapDelete("/flights/{id}", (int id, DatabaseConnection db) => Results.StatusCode(StatusCodes.Status501NotImplemented))
        .WithName("Del Flight")
        .WithOpenApi();
    }
}
