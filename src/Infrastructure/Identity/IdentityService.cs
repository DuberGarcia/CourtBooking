using Application.Common;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    public IdentityService(UserManager<IdentityUser<Guid>> userManager)
    {
        _userManager = userManager;
    }
    public async Task<bool> EmailExistsAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is not null;
    }
    public async Task<Guid> CreateUserAsync(string email, string password, string role)
    {
        var user = new IdentityUser<Guid>
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email
        };

        IdentityResult created = await _userManager.CreateAsync(user, password);
        if (!created.Succeeded) throw new ValidationException(ToMessage(created));

        IdentityResult addedToRole = await _userManager.AddToRoleAsync(user, role);
        if (!addedToRole.Succeeded) throw new InvalidOperationException(ToMessage(addedToRole));

        return user.Id;
    }
    private static string ToMessage(IdentityResult result)
        => string.Join(" ", result.Errors.Select(e => e.Description));

}