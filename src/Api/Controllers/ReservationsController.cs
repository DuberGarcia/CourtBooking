using Api.Extensions;
using Application.Auth.Constants;
using Application.Reservations.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public sealed record CreateReservationRequest(Guid CourtId, DateTime Start, DateTime End);

[ApiController]
[Authorize]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly CreateReservation _createReservation;
    private readonly CancelReservation _cancelReservation;

    public ReservationsController(CreateReservation createReservation, CancelReservation cancelReservation)
    {
        _createReservation = createReservation;
        _cancelReservation = cancelReservation;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReservationRequest request)
    {
        var memberId = User.GetUserId();

        var command = new CreateReservationCommand(request.CourtId, memberId, request.Start, request.End);
        var id = await _createReservation.ExecuteAsync(command);

        return Created($"/api/reservations/{id}", new { id });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.GetUserId();
        var isAdmin = User.IsInRole(Roles.Admin);

        var command = new CancelReservationCommand(id,userId,isAdmin);
        await _cancelReservation.ExecuteAsync(command);

        return NoContent();
    }
}