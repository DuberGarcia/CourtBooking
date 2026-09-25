using CourtBooking.Domain.Courts;
using CourtBooking.Domain.Reservations;

namespace CourtBooking.Application.Reservations;

public class CreateReservation
{
    private readonly ICourtRepository _courts;
    private readonly IReservationRepository _reservations;

    public CreateReservation(ICourtRepository courts, IReservationRepository reservations)
    {
        _courts = courts;
        _reservations = reservations;
    }

    public async Task<Guid> ExecuteAsync(Guid courtId, Guid userId, DateTime start, DateTime end)
    {
        var period = new TimeRange(start, end);

        var court = await _courts.GetByIdAsync(courtId);
        if (court is null)
            throw new KeyNotFoundException("La cancha no existe");

        var existing = await _reservations.GetByCourtAndDateAsync(courtId, DateOnly.FromDateTime(start));

        var reservation = court.Reserve(userId, period, existing);

        await _reservations.AddAsync(reservation);

        return reservation.Id;
    }
}