using Application.Auth.Interfaces;
using Application.Auth.Models;

namespace Application.Tests.Fakes;

public class FakeIdentityService : IIdentityService
{
    public sealed record CreatedUser(Guid Id, string Email, string Password, string Role);

    public List<CreatedUser> Users { get; } = new();

    public Task<bool> EmailExistsAsync(string email)
        => Task.FromResult(Users.Any(u => u.Email == email));

    public Task<Guid> CreateUserAsync(string email, string password, string role)
    {
        var id = Guid.NewGuid();
        Users.Add(new CreatedUser(id, email, password, role));
        return Task.FromResult(id);
    }

    public Task<UserInfo?> CheckCredentialsAsync(string email, string password)
    {
        var user = Users.FirstOrDefault(u => u.Email == email && u.Password == password);

        UserInfo? result = user is null
            ? null
            : new UserInfo(user.Id, user.Email, [user.Role]);

        return Task.FromResult(result);
    }
}