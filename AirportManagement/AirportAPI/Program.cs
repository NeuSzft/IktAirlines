using AirportAPI.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Text;

namespace AirportAPI;

public class DatabaseConnection() {
    public NpgsqlConnection Create() => new(Environment.GetEnvironmentVariable(Program.ConStrEnv) ?? throw new ApplicationException(Program.ConStrEnv + " environment variable is not defined"));

    public NpgsqlConnection CreateFrom(string connectionString) => new(connectionString);

    public NpgsqlConnection Open() {
        NpgsqlConnection con = Create();
        con.Open();
        return con;
    }
}

internal static class Program {
    public const string ConStrEnv = "CONNECTION_STRING";
    public const string PortEnv = "PORT";

    private static void Main(string[] args) {
        DatabaseConnection connection = new();
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.WebHost.UseUrls($"http://0.0.0.0:{Environment.GetEnvironmentVariable(PortEnv) ?? "5000"}");

        builder.Services.AddCors();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton(_ => connection);

        WebApplication app = builder.Build();

        app.Logger.LogInformation(Environment.GetEnvironmentVariable(ConStrEnv));

        app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(_ => true));

        if (app.Environment.IsDevelopment()) {
            app.UseSwagger(options => options.RouteTemplate = "/docs/{documentName}/swagger.json");
            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint("/docs/v1/swagger.json", "Airport API");
                options.RoutePrefix = "docs";
            });
        }

        app.Use(async (context, next) => {
            try {
                await next(context);
            } catch (NpgsqlException exception) {
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "text/plain";
                await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(exception.Message));
            }
        });

        app.UseRouting();

        app.MapFlights();
        app.MapCities();
        app.MapAirlines();
        app.MapOther();

        app.Run();
    }
}
