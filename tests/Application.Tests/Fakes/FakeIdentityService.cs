using Application.Common;

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
}