using Application.Auth.Models;

namespace Application.Auth.Interfaces;

public interface ITokenGenerator
{
    string Issue(UserInfo user);
}