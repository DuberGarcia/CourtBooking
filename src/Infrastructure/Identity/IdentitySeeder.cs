using Application.Auth.Constants;
using Application.Auth.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        string[] roles = [Roles.Admin, Roles.Customer];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = role });
        }
    }

    public static async Task SeedAdminAsybc(IServiceProvider services, string? email, string? password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

        using var scope = services.CreateScope();
        var accounts = scope.ServiceProvider.GetRequiredService<IIdentityService>();

        var normalized = email.Trim().ToLowerInvariant();

        if (await accounts.EmailExistsAsync(normalized)) return;

        await accounts.CreateUserAsync(normalized,password, Roles.Admin);
    }
}