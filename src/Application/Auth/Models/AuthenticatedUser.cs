namespace Application.Auth.Models;

public sealed record UserInfo(Guid Id, string Email, IReadOnlyList<string> Roles);