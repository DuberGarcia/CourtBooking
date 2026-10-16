namespace Domain.Reservations;

public interface IReservationRepository
{
    Task<IReadOnlyList<Reservation>> GetByCourtAsync(Guid courtId);
    Task AddAsync(Reservation reservation);
}