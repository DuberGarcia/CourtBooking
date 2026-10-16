using Application.Common;
using Domain.Common;
using Domain.Courts;
using Domain.Reservations;

namespace Application.Reservations;

public record CreateReservationCommand(Guid CourtId, Guid UserId, DateTime Start, DateTime End);
public class CreateReservation
{
    private readonly ICourtRepository _courts;
    private readonly IReservationRepository _reservations;

    public CreateReservation(ICourtRepository courts,IReservationRepository reservations)
    {
        _courts = courts;
        _reservations = reservations;
    }

    public async Task<Guid> ExecuteAsync(CreateReservationCommand command)
    {
        var range = new TimeRange(command.Start,command.End);
        
        var court = await _courts.GetByIdAsync(command.CourtId);
        if(court is null) throw new NotFoundException("No se encontro la cancha");

        var reservations = await _reservations.GetByCourtAsync(court.Id);

        var reservation = court.Reserve(command.UserId,range,reservations);

        await _reservations.AddAsync(reservation);

        return reservation.Id;
    }
}