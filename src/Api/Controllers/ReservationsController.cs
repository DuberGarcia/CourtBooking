using Api.Extensions;
using Application.Reservations.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public sealed record CreateReservationRequest(Guid CourtId, DateTime Start, DateTime End);

[ApiController]
[Authorize]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly CreateReservation _createReservation;

    public ReservationsController(CreateReservation createReservation)
    {
        _createReservation = createReservation;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReservationRequest request)
    {
        var memberId = User.GetUserId();

        var command = new CreateReservationCommand(request.CourtId, memberId, request.Start, request.End);
        var id = await _createReservation.ExecuteAsync(command);

        return Created($"/api/reservations/{id}", new { id });
    }
}