using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

namespace AirportAPI.Endpoints;

public static class Flights {
    public static IEnumerable<Models.Flight> GetFlights(this DatabaseConnection db) {
        const string query = """
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
        const string query = """
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

    public static IEnumerable<Models.Flight> GetFlight(this DatabaseConnection db, int id) {
        const string query = """
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

    public static IEnumerable<Models.FlightJoined> GetFlightJoined(this DatabaseConnection db, int id) {
        const string query = """
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
            WHERE flights.id = @Id;
        """;

        using var con = db.Open();
        return con.Query<Models.FlightJoined>(query, new { Id = id });
    }

    public static int PostFlight(this DatabaseConnection db, Models.Flight flight) {
        const string query = """
            INSERT INTO flights(airline_id, origin_id, destination_id, distance, flight_time, huf_per_km)
            VALUES (@AirlineId, @OriginId, @DestinationId, @Distance, @FlightTime, @HufPerKm);
        """;

        using var con = db.Open();
        return con.Execute(query, new { flight.AirlineId, flight.OriginId, flight.DestinationId, flight.Distance, flight.FlightTime, flight.HufPerKm });
    }

    public static int PutFlight(this DatabaseConnection db, int id, Models.Flight flight) {
        const string query = """
            UPDATE flights
            SET airline_id=@AirlineId, origin_id=@OriginId, destination_id=@DestinationId, distance=@Distance, flight_time=@FlightTime, huf_per_km=@HufPerKm
            WHERE flights.id = @Id;
        """;

        using var con = db.Open();
        return con.Execute(query, new { Id = id, flight.AirlineId, flight.OriginId, flight.DestinationId, flight.Distance, flight.FlightTime, flight.HufPerKm });
    }

    public static int DelFlight(this DatabaseConnection db, int id) {
        const string query = """
            DELETE FROM flights
            WHERE flights.id = @Id;
        """;

        using var con = db.Open();
        return con.Execute(query, new { Id = id });
    }

    public static void MapFlights(this WebApplication app) {
        app.MapGet("/flights", (DatabaseConnection db) => Results.Ok(db.GetFlights()))
        .WithName("Get Flights")
        .WithTags("Flight Endpoints")
        .WithOpenApi()
        .Produces<IEnumerable<Models.Flight>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/flights/joined", (DatabaseConnection db) => Results.Ok(db.GetFlightsJoined()))
        .WithName("Get Flights Joined")
        .WithTags("Flight Endpoints")
        .WithOpenApi()
        .Produces<IEnumerable<Models.FlightJoined>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/flights/{id}", (int id, DatabaseConnection db) => Results.Ok(db.GetFlight(id)))
        .WithName("Get Flight")
        .WithTags("Flight Endpoints")
        .WithOpenApi()
        .Produces<IEnumerable<Models.Flight>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/flights/{id}/joined", (int id, DatabaseConnection db) => Results.Ok(db.GetFlightJoined(id)))
        .WithName("Get Flight Joined")
        .WithTags("Flight Endpoints")
        .WithOpenApi()
        .Produces<IEnumerable<Models.FlightJoined>>(StatusCodes.Status200OK, "application/json");

        app.MapPost("/flights", (Models.Flight flight, DatabaseConnection db) => db.PostFlight(flight) > 0 ? Results.Created() : Results.BadRequest())
        .WithName("Post Flight")
        .WithTags("Flight Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/flights/{id}", (int id, Models.Flight flight, DatabaseConnection db) => db.PutFlight(id, flight) > 0 ? Results.Ok() : Results.NotFound())
        .WithName("Put Flight")
        .WithTags("Flight Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        app.MapDelete("/flights/{id}", (int id, DatabaseConnection db) => db.DelFlight(id) > 0 ? Results.Ok() : Results.NotFound())
        .WithName("Del Flight")
        .WithTags("Flight Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
