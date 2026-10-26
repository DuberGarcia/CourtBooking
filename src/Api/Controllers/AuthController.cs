using System.ComponentModel;
using Application.Auth;
using Application.Auth.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public sealed record RegisterUSerRequest(string email, string password);

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUser _registerUser;

    public AuthController(RegisterUser registerUser)
    {
        _registerUser = registerUser;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Create(RegisterUSerRequest request)
    {
        var command = new RegisterUserCommand(request.email,request.password);
        var id = await _registerUser.ExecuteAsync(command);

        return Created($"/api/users/{id}",new {id});
    }
}