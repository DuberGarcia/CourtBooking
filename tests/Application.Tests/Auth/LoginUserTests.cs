using Application.Auth.Constants;
using Application.Auth.Exceptions;
using Application.Auth.UseCases;
using Application.Common.Exceptions;
using Application.Tests.Fakes;

namespace Application.Tests.Auth;

public class LoginUserTests
{
    private readonly FakeIdentityService _identity = new();
    private readonly FakeTokenGenerator _tokens = new();
    private readonly LoginUser _login;
    private readonly Guid _anaId = Guid.NewGuid();

    public LoginUserTests()
    {
        _identity.Users.Add(new FakeIdentityService.CreatedUser(_anaId, "ana@mail.com", "Secreta123!", Roles.Customer));
        _login = new LoginUser(_identity, _tokens);
    }

    [Fact]
    public async Task Credenciales_correctas_devuelven_el_token()
    {
        var token = await _login.ExecuteAsync(new LoginUserCommand("ana@mail.com", "Secreta123!"));

        Assert.Equal($"token-for-{_anaId}", token);
    }

    [Fact]
    public async Task El_token_se_genera_para_el_usuario_correcto_con_sus_roles()
    {
        await _login.ExecuteAsync(new LoginUserCommand("ana@mail.com", "Secreta123!"));

        Assert.NotNull(_tokens.LastUser);
        Assert.Equal(_anaId, _tokens.LastUser.Id);
        Assert.Contains(Roles.Customer, _tokens.LastUser.Roles);
    }

    [Fact]
    public async Task Email_con_otro_formato_tambien_inicia_sesion()
    {
        var token = await _login.ExecuteAsync(new LoginUserCommand("  ANA@Mail.com ", "Secreta123!"));

        Assert.Equal($"token-for-{_anaId}", token);
    }

    [Fact]
    public async Task Contrasena_incorrecta_lanza_InvalidCredentialsException_y_no_genera_token()
    {
        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            _login.ExecuteAsync(new LoginUserCommand("ana@mail.com", "Otra456!")));

        Assert.Null(_tokens.LastUser);
    }

    [Fact]
    public async Task Email_inexistente_lanza_InvalidCredentialsException()
    {
        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            _login.ExecuteAsync(new LoginUserCommand("nadie@mail.com", "Secreta123!")));
    }

    [Fact]
    public async Task Email_inexistente_y_contrasena_incorrecta_dan_el_mismo_mensaje()
    {
        var wrongPassword = await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            _login.ExecuteAsync(new LoginUserCommand("ana@mail.com", "Otra456!")));

        var unknownEmail = await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            _login.ExecuteAsync(new LoginUserCommand("nadie@mail.com", "Secreta123!")));

        Assert.Equal(wrongPassword.Message, unknownEmail.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Email_vacio_lanza_ValidationException(string? email)
    {
        await Assert.ThrowsAsync<ValidationException>(() =>
            _login.ExecuteAsync(new LoginUserCommand(email!, "Secreta123!")));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Password_vacio_lanza_ValidationException(string? password)
    {
        await Assert.ThrowsAsync<ValidationException>(() =>
            _login.ExecuteAsync(new LoginUserCommand("ana@mail.com", password!)));
    }
}