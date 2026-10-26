using Application.Common;
using Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Errors;

public sealed class AppExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetails;

    public AppExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetails = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            DomainException => (StatusCodes.Status400BadRequest, "Regla de negocion no cumplida"),
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
            _ => (0, "")
        };

        if (status == 0) return false;

        httpContext.Response.StatusCode = status;

        return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = exception.Message   // el mensaje que escribiste en el dominio
            }
        });
    }
}