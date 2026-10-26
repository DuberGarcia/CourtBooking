using Domain.Courts;
using Domain.Reservations;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<CourtBookingDbContext>(options =>
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 0))));

        services.AddScoped<ICourtRepository, CourtRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();

        return services;
    }
}
