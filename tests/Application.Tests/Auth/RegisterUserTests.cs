using Application.Auth;
using Application.Common;
using Application.Tests.Fakes;

namespace Application.Tests.Auth;

public class RegisterUserTests
{
    private readonly FakeIdentityService _identity = new();
    private readonly RegisterUser _registerUser;

    public RegisterUserTests()
    {
        _registerUser = new RegisterUser(_identity);
    }

    [Fact]
    public async Task Registra_usuario_y_devuelve_su_id()
    {
        var id = await _registerUser.ExecuteAsync(new RegisterUserCommand("ana@mail.com", "Secreta123!"));

        var user = Assert.Single(_identity.Users);
        Assert.Equal(user.Id, id);
    }

    [Fact]
    public async Task Siempre_crea_usuarios_con_rol_Customer()
    {
        await _registerUser.ExecuteAsync(new RegisterUserCommand("ana@mail.com", "Secreta123!"));

        Assert.Equal(Roles.Customer, _identity.Users[0].Role);
    }

    [Fact]
    public async Task Normaliza_el_email_antes_de_guardarlo()
    {
        await _registerUser.ExecuteAsync(new RegisterUserCommand("  Ana@Mail.COM ", "Secreta123!"));

        Assert.Equal("ana@mail.com", _identity.Users[0].Email);
    }

    [Fact]
    public async Task Email_repetido_lanza_ConflictException_y_no_crea_otro_usuario()
    {
        await _registerUser.ExecuteAsync(new RegisterUserCommand("ana@mail.com", "Secreta123!"));

        await Assert.ThrowsAsync<ConflictException>(() =>
            _registerUser.ExecuteAsync(new RegisterUserCommand("ana@mail.com", "Otra456!")));

        Assert.Single(_identity.Users);
    }

    [Fact]
    public async Task Email_repetido_con_otro_formato_tambien_es_conflicto()
    {
        await _registerUser.ExecuteAsync(new RegisterUserCommand("ana@mail.com", "Secreta123!"));

        await Assert.ThrowsAsync<ConflictException>(() =>
            _registerUser.ExecuteAsync(new RegisterUserCommand(" ANA@mail.com ", "Otra456!")));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Email_vacio_lanza_ValidationException(string? email)
    {
        await Assert.ThrowsAsync<ValidationException>(() =>
            _registerUser.ExecuteAsync(new RegisterUserCommand(email!, "Secreta123!")));

        Assert.Empty(_identity.Users);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Password_vacio_lanza_ValidationException(string? password)
    {
        await Assert.ThrowsAsync<ValidationException>(() =>
            _registerUser.ExecuteAsync(new RegisterUserCommand("ana@mail.com", password!)));

        Assert.Empty(_identity.Users);
    }
}