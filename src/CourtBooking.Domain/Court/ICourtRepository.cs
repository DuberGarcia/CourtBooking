using CourtBooking.Domain.Reservations;

namespace CourtBooking.Domain.Courts;

public interface ICourtRepository
{
    Task<Court?> GetByIdAsync(Guid id);
}