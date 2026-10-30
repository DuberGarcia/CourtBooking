using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Courts;
using Domain.Reservations;

namespace Application.Reservations.UseCases;

public record CreateReservationCommand(Guid CourtId, Guid UserId, DateTime Start, DateTime End);
public class CreateReservation
{
    private readonly ICourtRepository _courts;
    private readonly IReservationRepository _reservations;
    private readonly ITransaction _transaction;

    public CreateReservation(ICourtRepository courts, IReservationRepository reservations,ITransaction transaction)
    {
        _courts = courts;
        _reservations = reservations;
        _transaction = transaction;
    }

    public async Task<Guid> ExecuteAsync(CreateReservationCommand command)
    {
        return await _transaction.RunAsync(async () =>
        {
            var range = new TimeRange(command.Start, command.End);

            var court = await _courts.GetByIdForUpdateAsync(command.CourtId);
            if (court is null) throw new NotFoundException("No se encontro la cancha");

            var reservations = await _reservations.GetConfirmedOverlappingAsync(court.Id, range);

            var reservation = court.Reserve(command.UserId, range, reservations);

            await _reservations.AddAsync(reservation);

            return reservation.Id;
        });
    }
}