using Application.Auth.Exceptions;
using Application.Auth.Interfaces;
using Application.Common.Exceptions;

namespace Application.Auth.UseCases;
public sealed record LoginUserCommand(string Email,string Password);
public class LoginUser
{
    private readonly IIdentityService _identityService;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginUser(IIdentityService identityService, ITokenGenerator tokenGenerator)
    {
        _identityService = identityService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<string> ExecuteAsync(LoginUserCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Email)) throw new ValidationException("El email es obligatorio");
        
        if (string.IsNullOrWhiteSpace(command.Password)) throw new ValidationException("La contraseña es obligatoria");

        var email = command.Email.Trim().ToLowerInvariant();

        var user = await _identityService.CheckCredentialsAsync(email,command.Password);

        if (user is null)
            throw new InvalidCredentialsException("Email o Contraseña incorrectos");
        
        return _tokenGenerator.Issue(user);
    }
}