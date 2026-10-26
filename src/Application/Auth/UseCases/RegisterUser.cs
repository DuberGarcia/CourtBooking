using Application.Auth.Constants;
using Application.Auth.Interfaces;
using Application.Common.Exceptions;

namespace Application.Auth.UseCases;

public sealed record RegisterUserCommand(string Email, string Password);

public class RegisterUser
{
    private readonly IIdentityService _identityService;

    public RegisterUser(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Guid> ExecuteAsync(RegisterUserCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Email)) throw new ValidationException("El email es obligatorio");

        if (string.IsNullOrWhiteSpace(command.Password)) throw new ValidationException("La contraseña es obligatoria");

        var email = command.Email.Trim().ToLowerInvariant();

        if (await _identityService.EmailExistsAsync(email)) throw new ConflictException($"Ya existe una cuenta con el email {email}");

        return await _identityService.CreateUserAsync(email, command.Password, Roles.Customer);
    }
}