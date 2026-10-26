using Application.Reservations.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public sealed record CreateReservationRequest(Guid CourtId, DateTime Start, DateTime End);

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly CreateReservation _createReservation;

    public ReservationsController(CreateReservation createReservation)
    {
        _createReservation = createReservation;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReservationRequest request, [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        var command = new CreateReservationCommand(request.CourtId, userId, request.Start, request.End);
        var id = await _createReservation.ExecuteAsync(command);

        return Created($"/api/reservations/{id}", new { id });
    }
}