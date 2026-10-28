using Application.Common.Exceptions;
using Domain.Reservations;

namespace Application.Reservations.UseCases;
public sealed record CancelReservationCommand(Guid ReservationId, Guid UserId, bool IsAdmin);
public class CancelReservation
{
    private const string ColombiaTimeZone = "America/Bogota";
    private readonly IReservationRepository _reservation;

    public CancelReservation(IReservationRepository reservationRepository)
    {
        _reservation = reservationRepository;
    }

    public async Task ExecuteAsync(CancelReservationCommand command)
    {
        var reservation = await _reservation.GetByIdAsync(command.ReservationId);
        if (reservation is null)
            throw new NotFoundException("La reserva no existe");
        
        var isOwner = reservation.UserId == command.UserId;
        if (!isOwner && !command.IsAdmin)
            throw new ForbiddenException("No puedes cancelar esta reserva");
        
        var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, ColombiaTimeZone);

        reservation.Cancelar(now);
        await _reservation.UpdateAsync(reservation);
    }
}