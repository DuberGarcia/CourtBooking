using Application.Auth.Interfaces;
using Domain.Courts;
using Domain.Reservations;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        JwtOptions jwtOptions)
    {
        services.AddDbContext<CourtBookingDbContext>(options =>
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 0))));

        services.AddScoped<ICourtRepository, CourtRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();

        services.AddIdentityCore<IdentityUser<Guid>>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<CourtBookingDbContext>();

        services.AddScoped<IIdentityService, IdentityService>();

        services.AddSingleton(jwtOptions);
        services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
