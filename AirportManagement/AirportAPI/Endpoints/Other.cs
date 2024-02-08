using AirportAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using System;
using System.Text.Json;

namespace AirportAPI.Endpoints;

public static class Other {
    public static int? GetCurrentSequenceId(this NpgsqlConnection connection, string table) {
        return connection.QueryFirstOrDefault<int?>($"SELECT last_value FROM {table}_id_seq;");
    }

    public static void MapOther(this WebApplication app) {
        app.MapPost("/price", async (DatabaseConnection db, HttpRequest request) => {
            try {
                Ticket? ticket = await JsonSerializer.DeserializeAsync<Ticket>(request.Body);
                if (ticket is null) return Results.BadRequest();

                using NpgsqlConnection connection = db.Open();
                FlightJoined? flight = connection.GetFlightJoined(ticket.Id);
                if (flight is null) return Results.NotFound();

                double price = 0;
                double baseCostPerPassenger = flight.Distance * flight.HufPerKm;

                double totalBaseCostAdult = baseCostPerPassenger * ticket.AdultsCount;
                double totalBaseCostChild = baseCostPerPassenger * ticket.ChildrenCount;

                double destinationPop = flight.DestinationCityPopulation;
                double tourismTaxRate = destinationPop < 2000000 ? 0.05 : destinationPop < 10000000 ? 0.075 : 0.10;

                double vatAdult = totalBaseCostAdult * 0.27;
                double vatChild = totalBaseCostChild * 0.27;
                double keroseneTax = flight.Distance * 0.10;
                double tourismTaxAdult = totalBaseCostAdult * tourismTaxRate;
                double tourismTaxChild = totalBaseCostChild * tourismTaxRate;

                double flightCostAdult = totalBaseCostAdult + vatAdult + keroseneTax + tourismTaxAdult;
                double flightCostChild = totalBaseCostChild + vatChild + keroseneTax + tourismTaxChild;

                if (ticket.AdultsCount + ticket.ChildrenCount > 10) {
                    flightCostAdult *= 0.90;
                    flightCostChild *= 0.90;
                }

                price += Math.Round(flightCostAdult + (flightCostChild * 0.8));
                return Results.Ok(price);
            } catch (JsonException) {
                return Results.BadRequest("Invalid JSON data.");
            }
        })
        .WithSummary("Calculate the price of a ticket.")
        .WithTags("Other Endpoints")
        .WithOpenApi()
        .Accepts<Ticket>("application/json")
        .Produces<int>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapGet("/ping", () => Results.Ok())
        .WithSummary("A quick way to check if the API is available.")
        .WithTags("Other Endpoints")
        .WithOpenApi()
        .Produces(StatusCodes.Status200OK);

        app.MapGet("/next-id/airlines", (DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            int? id = connection.GetCurrentSequenceId("airlines");
            return id is null ? Results.NotFound() : Results.Ok(id + 1);
        })
        .WithSummary("Get the next available id for an airline.")
        .WithTags("Other Endpoints")
        .WithOpenApi()
        .Produces<int>(StatusCodes.Status200OK, "text/plain")
        .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/next-id/cities", (DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            int? id = connection.GetCurrentSequenceId("cities");
            return id is null ? Results.NotFound() : Results.Ok(id + 1);
        })
        .WithSummary("Get the next available id for a city.")
        .WithTags("Other Endpoints")
        .WithOpenApi()
        .Produces<int>(StatusCodes.Status200OK, "text/plain")
        .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/next-id/flights", (DatabaseConnection db) => {
            using NpgsqlConnection connection = db.Open();
            int? id = connection.GetCurrentSequenceId("flights");
            return id is null ? Results.NotFound() : Results.Ok(id + 1);
        })
        .WithSummary("Get the next available id for a flight.")
        .WithTags("Other Endpoints")
        .WithOpenApi()
        .Produces<int>(StatusCodes.Status200OK, "text/plain")
        .Produces(StatusCodes.Status404NotFound);
    }
}
