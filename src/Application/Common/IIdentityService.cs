namespace Application.Common;
public interface IIdentityService
{
    Task<bool> EmailExistsAsync(string email);
    Task<Guid> CreateUserAsync(string email, string password, string role);
}