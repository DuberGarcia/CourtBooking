using Application.Auth.Models;

namespace Application.Auth.Interfaces;
public interface IIdentityService
{
    Task<bool> EmailExistsAsync(string email);
    Task<Guid> CreateUserAsync(string email, string password, string role);
    Task<UserInfo?> CheckCredentialsAsync(string email, string password);
}