using AirportAPI.Models;
using AirportAPI.Models.Testing;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace AirportAPI.Endpoints;

public static class Other {
    public static (Exception?, int) PerformOperations(this NpgsqlConnection connection, IEnumerable<Operation> operations) {
        int n = 0;

        try {
            foreach (Operation operation in operations) {
                {
                    if (operation is AddOperation<Airline> add) {
                        n += connection.PostAirline(add.Item);
                        continue;
                    }
                    if (operation is UpdateOperation<Airline> update) {
                        n += connection.PutAirline(update.Id, update.Item);
                        continue;
                    }
                    if (operation is RemoveOperation<Airline> remove) {
                        n += connection.DelAirline(remove.Id);
                        continue;
                    }
                }
                {
                    if (operation is AddOperation<City> add) {
                        n += connection.PostCity(add.Item);
                        continue;
                    }
                    if (operation is UpdateOperation<City> update) {
                        n += connection.PutCity(update.Id, update.Item);
                        continue;
                    }
                    if (operation is RemoveOperation<City> remove) {
                        n += connection.DelCity(remove.Id);
                        continue;
                    }
                }
                {
                    if (operation is AddOperation<Flight> add) {
                        n += connection.PostFlight(add.Item);
                        continue;
                    }
                    if (operation is UpdateOperation<Flight> update) {
                        n += connection.PutFlight(update.Id, update.Item);
                        continue;
                    }
                    if (operation is RemoveOperation<Flight> remove) {
                        n += connection.DelFlight(remove.Id);
                        continue;
                    }
                }

                throw new UnknownOperationException(operation);
            }
        } catch (UnknownOperationException e) {
            return (e, 0);
        } catch (NpgsqlException e) {
            return (e, 0);
        }

        return (null, n);
    }

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

        app.MapPatch("/modify", async (DatabaseConnection db, HttpRequest request) => {
            IEnumerable<Operation> operations;
            try {
                using JsonDocument document = await JsonDocument.ParseAsync(request.Body);
                operations = Operation.FromJson(document);
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }

            using NpgsqlConnection connection = db.Open();
            using NpgsqlTransaction transaction = connection.BeginTransaction();

            (Exception? exception, int affected) = PerformOperations(connection, operations);

            if (exception is null) {
                transaction.Commit();
                return Results.Ok(affected);
            }

            transaction.Rollback();
            return exception is UnknownOperationException ? Results.UnprocessableEntity(exception.Message) : Results.BadRequest(exception.Message);
        })
        .WithSummary("Perform all operations within a single transaction.")
        .WithTags("Other Endpoints")
        .WithOpenApi()
        .Accepts<IEnumerable<OperationInfo>>("application/json")
        .Produces<int>(StatusCodes.Status200OK, "text/plain")
        .Produces(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status422UnprocessableEntity, "text/plain");

        app.MapPatch("/modify/test", async (DatabaseConnection db, HttpRequest request) => {
            IEnumerable<Operation> operations;
            try {
                using JsonDocument document = await JsonDocument.ParseAsync(request.Body);
                operations = Operation.FromJson(document);
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }

            using NpgsqlConnection connection = db.Open();
            using NpgsqlTransaction transaction = connection.BeginTransaction();

            (Exception? exception, int affected) = PerformOperations(connection, operations);
            ModificationResults result = new(affected, connection.GetAirlines(), connection.GetCities(), connection.GetFlights());

            transaction.Rollback();

            if (exception is null)
                return Results.Ok(result);

            return exception is UnknownOperationException ? Results.UnprocessableEntity(exception.Message) : Results.BadRequest(exception.Message);
        })
        .WithSummary("Perform all operations within a single transaction then rollback.")
        .WithDescription("Returns the number of rows affected and the airlines, cities and flights tables in their pre-rollback state.")
        .WithTags("Other Endpoints")
        .WithOpenApi()
        .Accepts<IEnumerable<OperationInfo>>("application/json")
        .Produces<ModificationResults>(StatusCodes.Status200OK, "application/json")
        .Produces(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status422UnprocessableEntity, "text/plain");
    }
}
