using Application.Auth.Interfaces;
using Application.Auth.Models;

namespace Application.Tests.Fakes;

public class FakeTokenGenerator : ITokenGenerator
{
    public UserInfo? LastUser { get; private set; }

    public string Issue(UserInfo user)
    {
        LastUser = user;                  // recuerda para quién generó el token
        return $"token-for-{user.Id}";
    }
}