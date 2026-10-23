using Domain.Courts;
using Domain.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public class CourtBookingDbContextFactory : IDesignTimeDbContextFactory<CourtBookingDbContext>
{
    public CourtBookingDbContext CreateDbContext(string[] args)
    {
        // "??": si lo de la izquierda es null, usa lo de la derecha (aquí, lanzar error)
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DB_PORT")
            ?? throw new InvalidOperationException("Falta la variable de entorno DB_PORT.");
        var name = Environment.GetEnvironmentVariable("DB_NAME")
            ?? throw new InvalidOperationException("Falta la variable de entorno DB_NAME.");
        var user = Environment.GetEnvironmentVariable("DB_USER")
            ?? throw new InvalidOperationException("Falta la variable de entorno DB_USER.");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD")
                    ?? throw new InvalidOperationException("Falta la variable de entorno DB_PASSWORD.");
            
        // String interpolation: $"... {variable} ..."
        var connectionString = $"Server={host};Port={port};Database={name};User={user};Password={password}";

        var options = new DbContextOptionsBuilder<CourtBookingDbContext>()
            .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 0)))
            .Options;

        return new CourtBookingDbContext(options);
    }
}