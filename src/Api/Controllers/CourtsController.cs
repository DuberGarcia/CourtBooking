using Application.Auth.Constants;
using Application.Courts.UseCases;
using Application.Reservations.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
public sealed record CreateCourtRequest(string Name);

[ApiController]
[Route("api/courts")]
public class CourtsController : ControllerBase
{
    private readonly CreateCourt _createCourt;
    private readonly GetAvailability _getAvailability;

    public CourtsController(CreateCourt createCourt, GetAvailability getAvailability)
    {
        _createCourt = createCourt;
        _getAvailability = getAvailability;
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateCourtRequest request)
    {
        var id = await _createCourt.ExecuteAsync(new CreateCourtCommand(request.Name));

        return Created($"/api/courts/{id}", new { id });
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}/availability")]
    public async Task<IActionResult> GetCourtAvailability(Guid id, [FromQuery] DateOnly? date)
    {
        var query = new GetAvailabilityQuery(id,date);
        var availability = await _getAvailability.ExecuteAsync(query);

        return Ok(availability);
    }
}