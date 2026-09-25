using CourtBooking.Domain.Courts;
using CourtBooking.Domain.Reservations;
using CourtBooking.Infrastructure.Persistence;
using CourtBooking.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CourtBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 0))));

        services.AddScoped<ICourtRepository, CourtRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();

        return services;
    }
}