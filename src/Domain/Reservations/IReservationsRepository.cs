namespace Domain.Reservations;

public interface IReservationRepository
{
    Task<IReadOnlyList<Reservation>> GetByCourtAsync(Guid courtId);
    Task<Reservation?> GetByIdAsync(Guid id);
    Task AddAsync(Reservation reservation);
    Task UpdateAsync(Reservation reservation);
}