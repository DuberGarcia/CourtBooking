using System.Security.Claims;

namespace Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirst("sub")?.Value;

        if (!Guid.TryParse(sub, out var id))
            throw new InvalidOperationException("El token no tiene un claim 'sub' valido");
        
        return id;
    }
}