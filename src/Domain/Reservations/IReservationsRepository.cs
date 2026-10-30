using Domain.Common;

namespace Domain.Reservations;

public interface IReservationRepository
{
    Task<IReadOnlyList<Reservation>> GetConfirmedOverlappingAsync(Guid courtId, TimeRange range);
    Task<Reservation?> GetByIdAsync(Guid id);
    Task AddAsync(Reservation reservation);
    Task UpdateAsync(Reservation reservation);
}