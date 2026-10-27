using Application.Auth.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public sealed record RegisterUserRequest(string Email, string Password);
public sealed record LoginUserRequest(string Email, string Password);

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUser _registerUser;
    private readonly LoginUser _loginUser;

    public AuthController(RegisterUser registerUser, LoginUser loginUser)
    {
        _registerUser = registerUser;
        _loginUser = loginUser;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        var command = new RegisterUserCommand(request.Email, request.Password);
        var id = await _registerUser.ExecuteAsync(command);

        return Created($"/api/users/{id}", new { id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
        var command = new LoginUserCommand(request.Email, request.Password);
        var token = await _loginUser.ExecuteAsync(command);

        return Ok(new { accessToken = token });
    }
}