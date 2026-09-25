namespace CourtBooking.Domain.Reservations;

public interface IReservationRepository
{
    Task<List<Reservation>> GetByCourtAndDateAsync(Guid CourtId, DateOnly Start);
    Task AddAsync(Reservation reservation);
}