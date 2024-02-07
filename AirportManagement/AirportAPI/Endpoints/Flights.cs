using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using System.Collections.Generic;
using System.Linq;

namespace AirportAPI.Endpoints;

public static class Flights {
    public static IEnumerable<Models.Flight> GetFlights(this NpgsqlConnection connection) {
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

        return connection.Query<Models.Flight>(query);
    }

    public static IEnumerable<Models.FlightJoined> GetFlightsJoined(this NpgsqlConnection connection) {
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

        return connection.Query<Models.FlightJoined>(query);
    }

    public static Models.Flight? GetFlight(this NpgsqlConnection connection, int id) {
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
            WHERE flights.id = @Id;
        """;

        return connection.Query<Models.Flight>(query, new { Id = id }).FirstOrDefault();
    }

    public static Models.FlightJoined? GetFlightJoined(this NpgsqlConnection connection, int id) {
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

        return connection.Query<Models.FlightJoined>(query, new { Id = id }).FirstOrDefault();
    }

    public static int PostFlight(this NpgsqlConnection connection, Models.Flight flight) {
        const string query = """
            INSERT INTO flights(airline_id, origin_id, destination_id, distance, flight_time, huf_per_km)
            VALUES (@AirlineId, @OriginId, @DestinationId, @Distance, @FlightTime, @HufPerKm);
        """;

        return connection.Execute(query, new { flight.AirlineId, flight.OriginId, flight.DestinationId, flight.Distance, flight.FlightTime, flight.HufPerKm });
    }

    public static int PutFlight(this NpgsqlConnection connection, int id, Models.Flight flight) {
        const string query = """
            UPDATE flights
            SET airline_id=@AirlineId, origin_id=@OriginId, destination_id=@DestinationId, distance=@Distance, flight_time=@FlightTime, huf_per_km=@HufPerKm
            WHERE flights.id = @Id;
        """;

        return connection.Execute(query, new { Id = id, flight.AirlineId, flight.OriginId, flight.DestinationId, flight.Distance, flight.FlightTime, flight.HufPerKm });
    }

    public static int DelFlight(this NpgsqlConnection connection, int id) {
        const string query = """
            DELETE FROM flights
            WHERE flights.id = @Id;
        """;

        return connection.Execute(query, new { Id = id });
    }

    public static void MapFlights(this WebApplication app) {
        app.MapGet("/flights", (DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            return Results.Ok(connection.GetFlights());
        })
        .WithSummary("Get all flights.")
        .WithTags("Flights Endpoints")
        .WithOpenApi()
        .Produces<IEnumerable<Models.Flight>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/flights/joined", (DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            return Results.Ok(connection.GetFlightsJoined());
        })
        .WithSummary("Get all flights with the corresponding airline and city information joined to them.")
        .WithTags("Flights Endpoints")
        .WithOpenApi()
        .Produces<IEnumerable<Models.FlightJoined>>(StatusCodes.Status200OK, "application/json");

        app.MapGet("/flights/{id}", (int id, DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            var item = connection.GetFlight(id);
            return item is null ? Results.NotFound() : Results.Ok(item);
        })
        .WithSummary("Get a flight by id.")
        .WithTags("Flights Endpoints")
        .WithOpenApi()
        .Produces<IEnumerable<Models.Flight>>(StatusCodes.Status200OK, "application/json")
        .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/flights/{id}/joined", (int id, DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            var item = connection.GetFlightJoined(id);
            return item is null ? Results.NotFound() : Results.Ok(item);
        })
        .WithSummary("Get a flight by id with the corresponding airline and city information joined to them.")
        .WithTags("Flights Endpoints")
        .WithOpenApi()
        .Produces<IEnumerable<Models.FlightJoined>>(StatusCodes.Status200OK, "application/json")
        .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/flights", (Models.Flight flight, DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            return connection.PostFlight(flight) > 0 ? Results.Created() : Results.BadRequest();
        })
        .WithSummary("Add a new flight.")
        .WithTags("Flights Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/flights/{id}", (int id, Models.Flight flight, DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            return connection.PutFlight(id, flight) > 0 ? Results.Ok() : Results.NotFound();
        })
        .WithSummary("Overwrite an existing flight.")
        .WithTags("Flights Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        app.MapDelete("/flights/{id}", (int id, DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            return connection.DelFlight(id) > 0 ? Results.Ok() : Results.NotFound();
        })
        .WithSummary("Delete an existing flight.")
        .WithTags("Flights Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
